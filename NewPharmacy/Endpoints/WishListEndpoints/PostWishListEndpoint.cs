using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostWishListEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public PostWishListEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult> PostAddToWishList([FromBody] WishListItemUpsertDTO dto)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
            {
                return NotFound("Product was not found.");
            }

            var wishList = await _context.WishLists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.MyAppUserId == authInfo.UserId);

            if (wishList == null)
            {
                wishList = new WishList
                {
                    MyAppUserId = authInfo.UserId,
                    Date = DateTime.Now
                };

                _context.WishLists.Add(wishList);
                await _context.SaveChangesAsync();
            }

            var existingItem = await _context.WishListDetails
                .FirstOrDefaultAsync(wd => wd.WishListId == wishList.Id && wd.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                return Ok(new
                {
                    wishListId = wishList.Id,
                    wishListItemId = existingItem.Id,
                    productId = existingItem.ProductId,
                    alreadyExists = true
                });
            }

            var item = new WishListDetail
            {
                WishListId = wishList.Id,
                ProductId = product.Id,
                AddedAt = DateTime.Now
            };

            _context.WishListDetails.Add(item);
            wishList.Date = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                wishListId = wishList.Id,
                wishListItemId = item.Id,
                productId = item.ProductId,
                alreadyExists = false
            });
        }
    }
}
