using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteWishListEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public DeleteWishListEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpDelete("{productId}")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<IActionResult> DeleteFromWishList(int productId)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var wishList = await _context.WishLists
                .FirstOrDefaultAsync(w => w.MyAppUserId == authInfo.UserId);

            if (wishList == null)
            {
                return NotFound("Wishlist was not found.");
            }

            var item = await _context.WishListDetails
                .FirstOrDefaultAsync(wd => wd.WishListId == wishList.Id && wd.ProductId == productId);

            if (item == null)
            {
                return NotFound($"Wishlist item for product ID {productId} was not found.");
            }

            _context.WishListDetails.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
