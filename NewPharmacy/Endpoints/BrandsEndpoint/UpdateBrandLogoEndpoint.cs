using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.BrandsEndpoint
{
    [ApiController]
    [Route("api/brands")]
    public class UpdateBrandLogoEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureBlobService _blobService;

        public UpdateBrandLogoEndpoint(ApplicationDbContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpPut("{id:int}/logo")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> UpdateLogo(int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Odaberite logo brenda.");
            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Dozvoljeni su samo slikovni fajlovi.");
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Slika mora biti manja od 5 MB.");

            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null)
                return NotFound("Brend nije pronađen.");

            brand.LogoUrl = await _blobService.UploadImageAsync(file, "brand-images");
            await _context.SaveChangesAsync();
            return Ok(new { brand.Id, brand.LogoUrl });
        }
    }
}
