using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.AdvertisementEndpoints
{
    [ApiController]
    [Route("api/advertisements")]
    public class UpdateAdvertisementImageEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureBlobService _blobService;

        public UpdateAdvertisementImageEndpoint(ApplicationDbContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpPut("{id:int}/image")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> UpdateImage(int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Odaberite sliku reklame.");
            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Dozvoljeni su samo slikovni fajlovi.");
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Slika mora biti manja od 5 MB.");

            var advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.Id == id);
            if (advertisement == null)
                return NotFound("Reklama nije pronađena.");

            advertisement.imageURL = await _blobService.UploadImageAsync(file, "advertisement-images");
            await _context.SaveChangesAsync();
            return Ok(new { advertisement.Id, advertisement.imageURL });
        }
    }
}
