using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace Pharmacy.Api.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAdvertisementEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetAdvertisementEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var advertisements = await _context.Advertisements.ToListAsync();

            if (!advertisements.Any())
            {
                return NotFound("No advertisements found.");
            }
            return Ok(advertisements);
        }
    }
}

