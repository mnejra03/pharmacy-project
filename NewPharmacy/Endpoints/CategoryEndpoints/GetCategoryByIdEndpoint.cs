using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [Route("api/categories")]
    [ApiController]
    public class GetCategoryByIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetCategoryByIdEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound($"Category with Id {id} not found.");

            return Ok(category);
        }
    }
}
