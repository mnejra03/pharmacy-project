using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.RecipeEndpoints
{
    [ApiController]
    [Route("api/recipes")]
    public class GetAllRecipeEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetAllRecipeEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [MyAuthorization(isAdmin: true, isPharmacist: true, isCustomer: false)]
        public IActionResult GetAllRecipes()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || (!authInfo.IsPharmacist && !authInfo.IsAdmin))
            {
                return Forbid();
            }

            var recipes = _context.Recipes
                .Include(r => r.MyAppUser)
                .Select(r => new
                {
                    r.Id,
                    r.DateOfIssue,
                    r.DoctorFirstname,
                    r.DoctorLastname,
                    r.Status,
                    r.MyAppUserId,
                    UserFullname = r.MyAppUser.FirstName + " " + r.MyAppUser.LastName
                })
                .ToList();

            return Ok(recipes);
        }
    }
}
