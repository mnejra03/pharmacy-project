using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;

namespace NewPharmacy.Endpoints.BrandsEndpoint
{
    [Route("api")]
    [ApiController]
    public class GetBrandByIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetBrandByIdEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("brands/{id}")]
        public async Task<ActionResult<BrandDTO>> GetBrandById(int id)
        {
            var brand = await _context.Brands
            .Where(b => b.Id == id)
                .Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl,
                    Description = b.Description
                })
                .FirstOrDefaultAsync();

            if (brand == null)
                return NotFound();

            return Ok(brand);
        }
    }
}
