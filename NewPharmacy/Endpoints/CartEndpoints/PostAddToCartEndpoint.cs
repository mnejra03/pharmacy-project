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
    public class PostAddToCartEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public PostAddToCartEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult> PostAddToCart([FromBody] CartItemUpsertDTO dto)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            if (dto.Quantity < 1)
            {
                return BadRequest("Quantity must be at least 1.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
            {
                return NotFound("Product was not found.");
            }

            var cart = await _context.Carts
                .Where(c => c.MyAppUserId == authInfo.UserId && c.Status)
                .OrderByDescending(c => c.Date)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                cart = new Cart
                {
                    MyAppUserId = authInfo.UserId,
                    Date = DateTime.Now,
                    Status = true
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartDetail = await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.CartId == cart.Id && cd.ProductId == dto.ProductId);

            var unitPrice = product.IsDiscounted && product.DiscountedPrice > 0
                ? product.DiscountedPrice
                : product.Price;

            if (cartDetail == null)
            {
                cartDetail = new CartDetail
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    Price = unitPrice
                };

                _context.CartDetails.Add(cartDetail);
            }
            else
            {
                cartDetail.Quantity += dto.Quantity;
                cartDetail.Price = unitPrice;
            }

            cart.Date = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                cartId = cart.Id,
                cartDetailId = cartDetail.Id,
                productId = product.Id,
                quantity = cartDetail.Quantity,
                price = cartDetail.Price
            });
        }
    }
}
