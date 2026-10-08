using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetProductEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetProductEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();

            return Ok(products);
        }

    }
}
