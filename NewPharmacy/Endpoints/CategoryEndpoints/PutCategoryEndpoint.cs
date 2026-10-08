using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [Route("api/categories")]
    [ApiController]
    public class PutCategoryEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PutCategoryEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> PutCategory(int id, [FromBody] Category category)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (existingCategory == null)
                return NotFound($"Category with Id {id} not found.");

            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name cannot be empty.");

            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
