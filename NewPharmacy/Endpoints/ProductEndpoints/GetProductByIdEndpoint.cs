using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using System.Linq;

namespace NewPharmacy.Endpoints
{
    [Route("api/products")] 
    [ApiController]
    public class GetProductByIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetProductByIdEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        

    [HttpGet("{id}", Name ="GetProductById")]
    public IActionResult GetProductById(int id)
    {
        var product = _context.Products
            .Include(p => p.Category) 
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound("Product not found");
        }
        return Ok(product);
    }

}

}
