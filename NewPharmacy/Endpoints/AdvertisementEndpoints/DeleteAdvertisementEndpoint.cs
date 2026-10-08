using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace Pharmacy.Api.Endpoints
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeleteAdvertisementEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteAdvertisementEndpoint> _logger;

        public DeleteAdvertisementEndpoint(
            ApplicationDbContext context,
            ILogger<DeleteAdvertisementEndpoint> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
            if (advertisement == null)
            {
                return NotFound("Advertisement not found.");
            }

            try
            {
                _context.Advertisements.Remove(advertisement);
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete advertisement with id {AdvertisementId}.", id);
                return StatusCode(500, "An unexpected error occurred while deleting the advertisement.");
            }
        }
    }
}

