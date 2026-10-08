using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.ImageEndpoints
{
    [ApiController]
    [Route("api/images")]
    public class UploadImageEndpoint : ControllerBase
    {
        private readonly AzureBlobService _blobService;

        public UploadImageEndpoint(AzureBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost("products")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> UploadProductImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Odaberite sliku.");

            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Dozvoljeni su samo slikovni fajlovi.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Slika mora biti manja od 5 MB.");

            var imageUrl = await _blobService.UploadImageAsync(file, "product-images");
            return Ok(new { imageUrl });
        }
    }
}
