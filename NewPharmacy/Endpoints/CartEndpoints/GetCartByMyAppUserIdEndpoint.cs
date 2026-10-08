using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCartByMyAppUserIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetCartByMyAppUserIdEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartByMyAppUserId()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var cart = await _context.Carts
                .Where(c => c.MyAppUserId == authInfo.UserId && c.Status)
                .ToListAsync();

            if (cart.Count == 0)
            {
                return NotFound("Cart for the authenticated user was not found.");
            }

            return Ok(cart);
        }
    }
}
