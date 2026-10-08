using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

[ApiController]
[Route("api/[controller]")]
public class PutAdvertisementEndpoint : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PutAdvertisementEndpoint(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPut("{id}")]
    [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
    public async Task<IActionResult> UpdateAdvertisement(int id, [FromBody] Advertisement updatedAdvertisement)
    {
        if (updatedAdvertisement == null || id != updatedAdvertisement.Id)
        {
            return BadRequest("Invalid advertisement data or mismatched ID.");
        }

        var existingAdvertisement = await _context.Advertisements.FindAsync(id);
        if (existingAdvertisement == null)
        {
            return NotFound("Advertisement not found.");
        }

        existingAdvertisement.Title = updatedAdvertisement.Title;
        existingAdvertisement.imageURL = updatedAdvertisement.imageURL;
        

        await _context.SaveChangesAsync();

        return NoContent();
    }
}


