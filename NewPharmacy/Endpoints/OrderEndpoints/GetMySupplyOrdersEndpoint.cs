using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.OrderEndpoints
{
    [Route("api/orders/supply")]
    [ApiController]
    public class GetMySupplierOrdersEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetMySupplierOrdersEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsPharmacist)
                return authInfo == null ? Unauthorized() : Forbid();

            var userId = authInfo.UserId;

            var orders = await _context.Orders
                .Where(o => o.MyAppUserId == userId && o.IsSupplyOrder == true)
                .ToListAsync();

            var orderIds = orders.Select(o => o.Id).ToList();
            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId))
                .Include(od => od.Product)
                .ToListAsync();

            var result = orders.Select(order => new
            {
                order.Id,
                order.OrderDate,
                order.Status,
                OrderDetails = orderDetails
                    .Where(od => od.OrderId == order.Id)
                    .Select(od => new
                    {
                        od.Product?.Name,
                        od.Qty,
                        od.PricePerUnit
                    }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}
