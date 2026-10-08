using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    [Route("api/products")] // Obavezno postavi rutu na api/products
    [ApiController]
    public class GetSimilarProductsEndpoint : Controller
    {
        private readonly ApplicationDbContext _context;

        public GetSimilarProductsEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("similar")]
        public async Task<ActionResult<List<Product>>> GetSimilarProducts(string keyword, int excludeProductId)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required.");

            var products = await _context.Products
                .Where(p => p.Name.StartsWith(keyword) && p.Id != excludeProductId)
                .Take(3)
                .ToListAsync();

            return Ok(products);
        }



    }
}
