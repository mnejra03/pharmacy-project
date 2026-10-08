using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Data.Models;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostProductEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostProductEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> PostProduct([FromBody] ProductAddDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
                return BadRequest("Category not found.");

            if (dto.IsDiscounted && (dto.DiscountPercentage == null || dto.DiscountPercentage <= 0))
                return BadRequest("Discount percentage is required when product is discounted.");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                Picture = dto.Picture,
                CategoryId = dto.CategoryId,
                IsDiscounted = dto.IsDiscounted,
                DiscountPercentage = dto.DiscountPercentage,
                BrandId = dto.BrandId,
                DatumDodavanja = DateTime.Now
            };

            product.UpdateDiscountedPrice();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
