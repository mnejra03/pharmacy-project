using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.OrderDetailEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetOrderDetailsEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetOrderDetailsEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("by-order/{orderId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetDetailsByOrderId(int orderId)
        {
            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
            {
                return NotFound("Narudžba nije pronađena.");
            }

            var canAccess =
                authInfo.IsAdmin ||
                authInfo.IsPharmacist ||
                order.MyAppUserId == authInfo.UserId;

            if (!canAccess)
            {
                return Forbid();
            }

            var details = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            if (!details.Any())
                return NotFound("Nema stavki za ovu narudžbu.");

            return Ok(details);
        }
    }
}
