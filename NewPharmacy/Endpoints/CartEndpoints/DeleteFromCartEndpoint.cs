using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteFromCartEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public DeleteFromCartEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpDelete("{productId}")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<IActionResult> DeleteFromCart(int productId)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var activeCarts = await _context.Carts
                .Where(c => c.MyAppUserId == authInfo.UserId && c.Status)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            if (activeCarts.Count == 0)
            {
                return NotFound("Active cart was not found.");
            }

            var activeCartIds = activeCarts.Select(c => c.Id).ToList();

            var cartDetails = await _context.CartDetails
                .Where(cd => activeCartIds.Contains(cd.CartId) && cd.ProductId == productId)
                .ToListAsync();

            if (cartDetails.Count == 0)
            {
                return NotFound("The product with the given ID was not found in the user's cart.");
            }

            _context.CartDetails.RemoveRange(cartDetails);
            await _context.SaveChangesAsync();

            var emptyCartIds = new List<int>();
            foreach (var cart in activeCarts)
            {
                var hasAnyItemsLeft = await _context.CartDetails.AnyAsync(cd => cd.CartId == cart.Id);
                if (!hasAnyItemsLeft)
                {
                    emptyCartIds.Add(cart.Id);
                }
            }

            if (emptyCartIds.Count > 0)
            {
                var emptyCarts = activeCarts.Where(c => emptyCartIds.Contains(c.Id));
                _context.Carts.RemoveRange(emptyCarts);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
