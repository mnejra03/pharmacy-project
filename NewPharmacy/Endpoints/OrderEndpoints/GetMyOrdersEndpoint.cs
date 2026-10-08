using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.OrderEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMyOrdersEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetMyOrdersEndpoint> _logger;
        private readonly MyAuthService _authService;

        public GetMyOrdersEndpoint(
            ApplicationDbContext context,
            ILogger<GetMyOrdersEndpoint> logger,
            MyAuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyCustomerOrders()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var userId = authInfo.UserId;
            _logger.LogInformation("Getting customer orders for authenticated user {UserId}", userId);

            var orders = await _context.Orders
                .Where(o => o.MyAppUserId == userId && o.IsSupplyOrder == false && !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Status,
                    o.TotalPrice,
                    o.PaymentMethod,
                    o.ShippingAddress,
                    OrderDetails = _context.OrderDetails
                        .Where(od => od.OrderId == o.Id)
                        .Select(od => new
                        {
                            od.ProductId,
                            ProductName = od.Product.Name,
                            od.Qty,
                            od.PricePerUnit
                        })
                        .ToList()
                })
                .ToListAsync();

            _logger.LogInformation("Found {OrderCount} customer orders for authenticated user {UserId}", orders.Count, userId);
            return Ok(orders);
        }
    }
}
