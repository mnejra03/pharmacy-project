using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutProductEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PutProductEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id != id)
                return BadRequest("ID mismatch.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound("Product not found.");

            if (dto.IsDiscounted && (dto.DiscountPercentage == null || dto.DiscountPercentage <= 0))
                return BadRequest("Discount percentage is required when product is discounted.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.QuantityInStock = dto.QuantityInStock;
            product.Picture = dto.Picture;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;
            product.IsDiscounted = dto.IsDiscounted;
            product.DiscountPercentage = dto.IsDiscounted ? dto.DiscountPercentage : null;

            product.UpdateDiscountedPrice();

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
