using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.ReviewEndpoints
{
    [ApiController]
    [Route("api")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public ReviewsController(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("add-review")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public IActionResult AddReview([FromBody] ReviewDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn || !authInfo.IsCustomer)
            {
                return Unauthorized();
            }

            var review = new Review
            {
                UserName = string.IsNullOrWhiteSpace(authInfo.FullName) ? authInfo.Username : authInfo.FullName,
                Rating = dto.Rating,
                Text = dto.Text,
                ProductId = dto.ProductId
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok(new { message = "Review added successfully" });
        }
    }
}
