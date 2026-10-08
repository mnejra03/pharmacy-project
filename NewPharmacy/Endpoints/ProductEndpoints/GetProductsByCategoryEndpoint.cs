using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
        [Route("api/GetProductsByCategory")]
        [ApiController]
        public class GetProductsByCategoryEndpoint : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public GetProductsByCategoryEndpoint(ApplicationDbContext context)
            {
                _context = context;
            }

            [HttpGet("{categoryId}")]
            public IActionResult GetProductsByCategory(int categoryId)
            {
                var products = _context.Products
                    .Where(p => p.Category.Id == categoryId)
                    .ToList();

                return Ok(products);
            }
        }
}

