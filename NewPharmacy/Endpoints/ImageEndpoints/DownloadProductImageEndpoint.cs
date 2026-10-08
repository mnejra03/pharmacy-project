using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints.ImageEndpoints;

[ApiController]
[Route("api/products")]
public class DownloadProductImageEndpoint : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AzureBlobService _blobService;

    public DownloadProductImageEndpoint(ApplicationDbContext context, AzureBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> Download(int id)
    {
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound("Product not found.");
        if (string.IsNullOrWhiteSpace(product.Picture)) return NotFound("Product image not found.");

        try
        {
            var image = await _blobService.DownloadImageAsync(product.Picture);
            return File(image.Content, image.ContentType, image.FileName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Azure.RequestFailedException)
        {
            return NotFound("Product image could not be found in storage.");
        }
    }
}
