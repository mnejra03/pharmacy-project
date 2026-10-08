using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.NotificationEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetNotificationEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetNotificationEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<FullNotificationDTO>>> GetNotifications()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var userId = authInfo.UserId;

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.MyAppUserId == userId)
                .Include(n => n.Order)
                    .ThenInclude(o => o!.OrderDetails)
                        .ThenInclude(od => od.Product)
                .OrderByDescending(n => n.Time)
                .ToListAsync();

            var result = notifications.Select(n => new FullNotificationDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Time = n.Time,
                Read = n.Read,
                Type = n.Type,
                MyAppUserId = n.MyAppUserId,
                SenderId = n.SenderId,
                OrderId = n.OrderId ?? 0,
                Order = n.Order == null
                    ? null
                    : new NotificationOrderDTO
                    {
                        Id = n.Order.Id,
                        OrderDate = n.Order.OrderDate,
                        Status = n.Order.Status,
                        TotalPrice = n.Order.TotalPrice,
                        PaymentMethod = n.Order.PaymentMethod,
                        ShippingAddress = n.Order.ShippingAddress,
                        MyAppUserId = n.Order.MyAppUserId,
                        IsSupplyOrder = n.Order.IsSupplyOrder
                    },
                OrderDetails = n.Order?.OrderDetails?
                    .Select(od => new NotificationOrderDetailDTO
                    {
                        Id = od.Id,
                        Qty = od.Qty,
                        PricePerUnit = od.PricePerUnit,
                        Product = od.Product == null
                            ? null
                            : new NotificationProductDTO
                            {
                                Id = od.Product.Id,
                                Name = od.Product.Name
                            }
                    })
                    .ToList() ?? new List<NotificationOrderDetailDTO>()
            }).ToList();

            return Ok(result);
        }
    }
}
