using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    [Route("api/[controller]")]
    [Route("api/products")]
    [ApiController]
    public class GetProductsPagedEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetProductsPagedEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetProductsPaged(
            [FromQuery] string? name,
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] decimal? minPrice,
            [FromQuery] bool? onlyDiscounted,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (onlyDiscounted == true)
                query = query.Where(p => p.IsDiscounted);

           
            var totalCount = await query.CountAsync();

            
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.QuantityInStock,
                    p.Picture,
                    p.CategoryId,
                    p.BrandId,
                    p.IsDiscounted,
                    p.DiscountPercentage,
                    p.DiscountedPrice
                })
                .ToListAsync();

            return Ok(new PagedResult<object>
            {
                Items = items.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}
