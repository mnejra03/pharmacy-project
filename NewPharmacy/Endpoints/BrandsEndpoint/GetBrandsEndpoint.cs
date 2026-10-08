using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints
{

    [ApiController]
    [Route("api/products/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("products/brands")]
        public IActionResult GetBrands()
        {
            var brands = _context.Brands
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.LogoUrl,
                    b.Description
                })
                .ToList();
            return Ok(brands);
        }
    }
}


