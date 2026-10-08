using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [Route("api/categories")]
    [ApiController]
    public class PostCategoryEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostCategoryEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name cannot be empty.");

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                actionName: nameof(GetCategoryByIdEndpoint.GetCategoryById),
                controllerName: nameof(GetCategoryByIdEndpoint).Replace("Endpoint", string.Empty),
                routeValues: new { id = category.Id },
                value: category);
        }
    }
}
