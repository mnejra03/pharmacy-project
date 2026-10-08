using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.CartEndpoints
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetActiveCartEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetActiveCartEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("active")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<IActionResult> GetActiveCart()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized("Korisnik nije prijavljen.");
            }

            var activeCartIds = await _context.Carts
                .Where(c => c.MyAppUserId == authInfo.UserId && c.Status)
                .OrderByDescending(c => c.Date)
                .Select(c => c.Id)
                .ToListAsync();

            if (activeCartIds.Count == 0)
            {
                return Ok(new List<object>());
            }

            var cartDetails = await _context.CartDetails
                .Where(cd => activeCartIds.Contains(cd.CartId))
                .Include(cd => cd.Product)
                .GroupBy(cd => new
                {
                    cd.ProductId,
                    ProductName = cd.Product.Name,
                    ProductDescription = cd.Product.Description,
                    ProductPicture = cd.Product.Picture
                })
                .Select(group => new
                {
                    group.Key.ProductId,
                    group.Key.ProductName,
                    group.Key.ProductDescription,
                    group.Key.ProductPicture,
                    Quantity = group.Sum(x => x.Quantity),
                    Price = group.OrderByDescending(x => x.Cart.Date).Select(x => x.Price).FirstOrDefault(),
                    Total = group.OrderByDescending(x => x.Cart.Date).Select(x => x.Price).FirstOrDefault() * group.Sum(x => x.Quantity)
                })
                .ToListAsync();

            return Ok(cartDetails);
        }
    }
}
