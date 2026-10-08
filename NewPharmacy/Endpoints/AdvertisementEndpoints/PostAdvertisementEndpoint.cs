using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace Pharmacy.Api.Endpoints
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostAdvertisementEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostAdvertisementEndpoint> _logger;

        public PostAdvertisementEndpoint(
            ApplicationDbContext context,
            ILogger<PostAdvertisementEndpoint> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> CreateAdvertisement([FromBody] Advertisement advertisement)
        {
            if (advertisement == null)
            {
                return BadRequest("Advertisement data is required.");
            }

            if (string.IsNullOrEmpty(advertisement.Title) || string.IsNullOrEmpty(advertisement.imageURL))
            {
                return BadRequest("Title and ImageURL are required.");
            }

            try
            {
                await _context.Advertisements.AddAsync(advertisement);
                await _context.SaveChangesAsync();

                return Created("api/GetAdvertisement", advertisement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create advertisement.");
                return StatusCode(500, "An unexpected error occurred while creating the advertisement.");
            }
        }
    }
}

