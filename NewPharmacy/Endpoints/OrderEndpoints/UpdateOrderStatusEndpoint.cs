using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace NewPharmacy.Endpoints.OrderEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateOrderStatusEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateOrderStatusEndpoint> _logger;
        private readonly MyAuthService _authService;

        public UpdateOrderStatusEndpoint(
            ApplicationDbContext context,
            ILogger<UpdateOrderStatusEndpoint> logger,
            MyAuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }

        [HttpPut("{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusRequestDTO request)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            if (!authInfo.IsAdmin && !authInfo.IsPharmacist)
            {
                return Forbid();
            }

            _logger.LogInformation("Updating order {OrderId} status to {NewStatus}.", orderId, request.NewStatus);

            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} was not found.", orderId);
                return NotFound("Narudžba nije pronađena.");
            }

            if (order.Status == "Delivered" || order.Status == "Cancelled")
            {
                _logger.LogWarning("Cannot change status for order {OrderId} because it is already {OrderStatus}.", orderId, order.Status);
                return BadRequest($"Narudžba je već {order.Status}. Status se ne može mijenjati.");
            }

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(request.NewStatus))
            {
                _logger.LogWarning("Rejected invalid order status '{NewStatus}' for order {OrderId}.", request.NewStatus, orderId);
                return BadRequest($"Nevažeći status. Dozvoljeni statusi: {string.Join(", ", validStatuses)}");
            }

            var oldStatus = order.Status;
            order.Status = request.NewStatus;

            var notification = new Notification
            {
                Title = "Status narudžbe promijenjen",
                Message = GetStatusChangeMessage(order.Id, oldStatus, request.NewStatus),
                MyAppUserId = order.MyAppUserId,
                OrderId = order.Id,
                Time = DateTime.UtcNow,
                Type = "order_status_change",
                SenderId = null 
            };
            _context.Notifications.Add(notification);

            if (order.IsSupplyOrder && request.NewStatus == "Delivered")
            {
                _logger.LogInformation("Updating stock for delivered supply order {OrderId}.", orderId);

                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                foreach (var detail in orderDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.QuantityInStock += detail.Qty;
                        _logger.LogInformation(
                            "Adjusted stock for product {ProductName} by {Quantity}. New stock: {QuantityInStock}.",
                            product.Name,
                            detail.Qty,
                            product.QuantityInStock);
                    }
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Order {OrderId} status updated from {OldStatus} to {NewStatus}.",
                orderId,
                oldStatus,
                request.NewStatus);

            return Ok(new
            {
                Message = "Status narudžbe ažuriran i obavijest poslana.",
                OrderId = orderId,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus
            });
        }

        private string GetStatusChangeMessage(int orderId, string oldStatus, string newStatus)
        {
            return newStatus switch
            {
                "Processing" => $"Vaša narudžba #{orderId} je primljena i trenutno se obrađuje.",
                "Shipped" => $"Vaša narudžba #{orderId} je poslata i uskoro će biti isporučena.",
                "Delivered" => $"Vaša narudžba #{orderId} je uspješno isporučena. Hvala na kupovini!",
                "Cancelled" => $"Vaša narudžba #{orderId} je otkazana.",
                _ => $"Status vaše narudžbe #{orderId} je promijenjen na '{newStatus}'."
            };
        }
    }

    public class UpdateOrderStatusRequestDTO
    {
        public string NewStatus { get; set; } = string.Empty;
    }
}

