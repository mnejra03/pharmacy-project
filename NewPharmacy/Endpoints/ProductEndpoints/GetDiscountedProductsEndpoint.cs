using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace PharmacyAPI.ProductEndpoints
{
    [Route("api/products/discounted")]
    [ApiController]
    public class GetDiscountedProductsEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetDiscountedProductsEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscountedProducts()
        {
            var discountedProducts = await _context.Products
                .Where(p => p.IsDiscounted && p.DiscountPercentage.HasValue && p.DiscountedPrice < p.Price)
                .ToListAsync();

            if (!discountedProducts.Any())
            {
                return NotFound("Nema trenutno sniženih proizvoda.");
            }

            return Ok(discountedProducts);
        }

    }
}

