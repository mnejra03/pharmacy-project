using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using Microsoft.EntityFrameworkCore; 

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    [Route("api")]
    [ApiController]
    public class GetProductsByBrandEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetProductsByBrandEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-brand/{brandId}")]
        public IActionResult GetProductsByBrand(int brandId)
        {
            var products = _context.Products
                .Where(p => p.BrandId == brandId)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Picture
                })
                .ToList();

            return Ok(products);
        }

    }
}
