using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [Route("api/categories")]
    [ApiController]
    public class DeleteCategoryEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeleteCategoryEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound($"Category with Id {id} not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
