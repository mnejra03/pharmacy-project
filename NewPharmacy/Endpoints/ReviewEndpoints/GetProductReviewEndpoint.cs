using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;

namespace NewPharmacy.Endpoints.ReviewEndpoints
{
    [ApiController]
    [Route("api")]
    public class GetProductReviewEndpoint : Controller
    {
        private readonly ApplicationDbContext _context;

        public GetProductReviewEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews(int productId)
        {
            
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviews == null)
            {
                return Ok(new List<ReviewDTO>());
            }


           
            var reviewDtos = reviews.Select(r => new ReviewDTO
            {
                UserName = r.UserName,
                Text = r.Text,
                Rating = r.Rating,
                ProductId = r.ProductId
            }).ToList();

            return Ok(reviewDtos);
        }
    }
}
