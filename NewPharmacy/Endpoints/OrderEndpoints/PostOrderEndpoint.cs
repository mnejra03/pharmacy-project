using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Data.Models;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Services;
using NewPharmacy.SignalR;
using Stripe;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostOrderEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderPricingService _orderPricingService;
        private readonly MyAuthService _authService;
        private readonly IHubContext<ChatHub> _hubContext;

        public PostOrderEndpoint(
            ApplicationDbContext context,
            OrderPricingService orderPricingService,
            MyAuthService authService,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _orderPricingService = orderPricingService;
            _authService = authService;
            _hubContext = hubContext;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult<Order>> PostOrder(OrderCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var user = await _context.MyAppUsers.FirstOrDefaultAsync(u => u.ID == authInfo.UserId);
            if (user == null)
            {
                return BadRequest("Authenticated user was not found.");
            }

            decimal verifiedTotal;
            try
            {
                verifiedTotal = await _orderPricingService.CalculateTotalAsync(dto.Items, dto.DeliveryMethod);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            var paymentValidationError = await ValidatePaymentAsync(dto, verifiedTotal);
            if (paymentValidationError is not null)
            {
                return paymentValidationError;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = CreateOrder(dto, user, verifiedTotal);
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderItemsResult = await CreateOrderItemsAndUpdateStockAsync(order, dto.Items);
                if (orderItemsResult is not null)
                {
                    await transaction.RollbackAsync();
                    return orderItemsResult;
                }

                await CreatePharmacistNotificationsAsync(order, user);

                await transaction.CommitAsync();

                return Ok(new
                {
                    order.Id,
                    order.OrderDate,
                    order.Status,
                    order.TotalPrice,
                    order.PaymentMethod,
                    order.ShippingAddress,
                    order.MyAppUserId,
                    order.IsSupplyOrder
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<ActionResult<Order>?> ValidatePaymentAsync(OrderCreateDTO dto, decimal verifiedTotal)
        {
            if (dto.PaymentMethod != "card")
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentToken))
            {
                return BadRequest("Missing payment token.");
            }

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            try
            {
                paymentIntent = await paymentIntentService.GetAsync(dto.PaymentToken);
            }
            catch (StripeException ex)
            {
                return StatusCode(502, ex.StripeError?.Message ?? ex.Message);
            }

            if (paymentIntent.Status != "succeeded")
            {
                return BadRequest("Card payment was not completed successfully.");
            }

            if (paymentIntent.AmountReceived != _orderPricingService.ToMinorUnits(verifiedTotal))
            {
                return BadRequest("Paid amount does not match the server-calculated order total.");
            }

            return null;
        }

        private Order CreateOrder(OrderCreateDTO dto, MyAppUser user, decimal verifiedTotal)
        {
            var fullAddress = $"{dto.Address}, {dto.City}, {dto.PostalCode}, {dto.Country}";

            return new Order
            {
                MyAppUserId = user.ID,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = verifiedTotal,
                PaymentMethod = dto.PaymentMethod,
                PaymentToken = dto.PaymentMethod == "card" ? dto.PaymentToken : null,
                ShippingAddress = fullAddress,
                IsSupplyOrder = false
            };
        }

        private async Task<ActionResult<Order>?> CreateOrderItemsAndUpdateStockAsync(Order order, List<OrderItemDTO> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    return BadRequest($"Product with ID {item.ProductId} does not exist.");
                }

                if (product.QuantityInStock < item.Qty)
                {
                    return BadRequest($"Insufficient stock for product with ID {item.ProductId}.");
                }

                var unitPrice = product.IsDiscounted && product.DiscountedPrice > 0
                    ? product.DiscountedPrice
                    : product.Price;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Qty = item.Qty,
                    PricePerUnit = unitPrice
                };

                _context.OrderDetails.Add(orderDetail);
                product.QuantityInStock -= item.Qty;
            }

            await _context.SaveChangesAsync();
            return null;
        }

        private async Task CreatePharmacistNotificationsAsync(Order order, MyAppUser user)
        {
            var pharmacists = await _context.MyAppUsers
                .Where(u => u.IsPharmacist)
                .ToListAsync();
            var createdNotifications = new List<Notification>();

            foreach (var pharmacist in pharmacists)
            {
                var notification = new Notification
                {
                    Title = "Nova narudzba",
                    Message = $"Kreirana je nova narudzba ID: {order.Id} od korisnika {user.FirstName} {user.LastName}.",
                    MyAppUserId = pharmacist.ID,
                    OrderId = order.Id,
                    Time = DateTime.UtcNow,
                    Type = "new_order",
                    SenderId = user.ID
                };
                _context.Notifications.Add(notification);
                createdNotifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            foreach (var notification in createdNotifications)
            {
                await _hubContext.Clients
                    .Group(notification.MyAppUserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.Id,
                        title = notification.Title,
                        message = notification.Message,
                        senderId = notification.SenderId,
                        time = notification.Time,
                        type = notification.Type
                    });
            }
        }
    }
}
