using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetWishListEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetWishListEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("my")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult> GetMyWishList()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var items = await _context.WishListDetails
                .Where(wd => wd.WishList != null && wd.WishList.MyAppUserId == authInfo.UserId)
                .Include(wd => wd.Product)
                .OrderByDescending(wd => wd.AddedAt)
                .Select(wd => new
                {
                    wd.Id,
                    wd.ProductId,
                    wd.AddedAt,
                    ProductName = wd.Product != null ? wd.Product.Name : string.Empty,
                    ProductDescription = wd.Product != null ? wd.Product.Description : string.Empty,
                    ProductPicture = wd.Product != null ? wd.Product.Picture : string.Empty,
                    Price = wd.Product != null ? wd.Product.Price : 0m,
                    DiscountedPrice = wd.Product != null ? wd.Product.DiscountedPrice : 0m,
                    IsDiscounted = wd.Product != null && wd.Product.IsDiscounted
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
