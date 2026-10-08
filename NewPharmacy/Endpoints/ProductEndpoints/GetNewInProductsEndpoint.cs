using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace PharmacyAPI.ProductEndpoints
{
    [Route("api/products/newInProducts")]
    [Route("api/products/new-in")]
    [ApiController]
    public class GetLatestProductsEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetLatestProductsEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewInProducts()
        {
            var newInProducts = await _context.Products
                .OrderByDescending(p => p.DatumDodavanja) 
                .Take(4)
                .ToListAsync();

            if (!newInProducts.Any())
            {
                return NotFound("Nema proizvoda.");
            }

            return Ok(newInProducts);
        }
    }
}
