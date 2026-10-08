using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.RecipeEndpoints
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipeEndpoints : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public RecipeEndpoints(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult<List<Recipe>>> AddAndGetRecipes([FromForm] RecipeUploadDto dto)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var newRecipe = new Recipe
            {
                DateOfIssue = DateTime.Now,
                DoctorFirstname = dto.DoctorFirstname,
                DoctorLastname = dto.DoctorLastname,
                MyAppUserId = authInfo.UserId,
                Status = "Pending"
            };

            if (dto.Scan != null)
            {
                using var ms = new MemoryStream();
                await dto.Scan.CopyToAsync(ms);
                newRecipe.Scan = ms.ToArray();
            }

            _context.Recipes.Add(newRecipe);
            await _context.SaveChangesAsync();

            var userRecipes = await _context.Recipes
                .Where(r => r.MyAppUserId == authInfo.UserId)
                .ToListAsync();

            return Ok(userRecipes);
        }

        [HttpGet("my")]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<ActionResult<List<Recipe>>> GetMyRecipes()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var recipes = await _context.Recipes
                .Where(r => r.MyAppUserId == authInfo.UserId)
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpPut("{id}/status")]
        [MyAuthorization(isAdmin: false, isPharmacist: true, isCustomer: false)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class UpdateStatusDto
        {
            public string Status { get; set; }
        }

        public class RecipeUploadDto
        {
            public string DoctorFirstname { get; set; }
            public string DoctorLastname { get; set; }
            public IFormFile? Scan { get; set; }
        }
    }
}
