using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using System.Linq;

namespace NewPharmacy.Endpoints
{
    [Route("api/getCategory")]
    [Route("api/categories")]
    [ApiController]
    public class GetCategoryEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetCategoryEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategory()
        {
            var category = _context.Categories.ToList();
            return Ok(category);
        }
    }
}
