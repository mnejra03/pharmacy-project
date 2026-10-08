using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.OrderEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateSupplyOrderStatusEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public UpdateSupplyOrderStatusEndpoint(
            ApplicationDbContext context,
            MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPut("{orderId}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> UpdateSupplyOrderStatus(
            int orderId,
            [FromBody] UpdateSupplyOrderStatusRequestDTO request)
        {
            if (!_authService.GetAuthInfo().IsAdmin)
            {
                return Forbid();
            }

            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound("Narudžba nije pronađena.");
            }

            if (!order.IsSupplyOrder)
            {
                return BadRequest("Ova narudžba nije narudžba za dopunu zaliha.");
            }

            if (order.Status == "Approved" || order.Status == "Rejected")
            {
                return BadRequest($"Narudžba je već {order.Status}. Status se ne može mijenjati.");
            }

            var validStatuses = new[] { "Pending", "Approved", "Rejected" };
            if (!validStatuses.Contains(request.NewStatus))
            {
                return BadRequest($"Nevažeći status. Dozvoljeni statusi: {string.Join(", ", validStatuses)}");
            }

            var oldStatus = order.Status;
            order.Status = request.NewStatus;

            if (request.NewStatus == "Approved")
            {
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                foreach (var detail in orderDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.QuantityInStock += detail.Qty;
                    }
                }
            }

            var notification = new Notification
            {
                Title = "Status narudžbe za dopunu zaliha",
                Message = GetSupplyOrderStatusMessage(order.Id, request.NewStatus),
                MyAppUserId = order.MyAppUserId,
                OrderId = order.Id,
                Time = DateTime.UtcNow,
                Type = "supply_order_status",
                SenderId = null
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Status narudžbe za dopunu zaliha ažuriran.",
                OrderId = orderId,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus
            });
        }

        private string GetSupplyOrderStatusMessage(int orderId, string newStatus)
        {
            return newStatus switch
            {
                "Approved" => $"Vaša narudžba #{orderId} za dopunu zaliha je odobrena. Zalihe su ažurirane.",
                "Rejected" => $"Vaša narudžba #{orderId} za dopunu zaliha je odbijena.",
                _ => $"Status vaše narudžbe #{orderId} je promijenjen na '{newStatus}'."
            };
        }
    }

    public class UpdateSupplyOrderStatusRequestDTO
    {
        public string NewStatus { get; set; } = string.Empty;
    }
}
