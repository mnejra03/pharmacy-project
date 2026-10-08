using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Services;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.AuthEndpoint
{
    [Route("auth")]
    [ApiController]
    public class AuthRegisterEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public AuthRegisterEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDTO request)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.MyAppUsers
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (existingUser != null)
                return BadRequest(new { Message = "Username already exists" });

            var newUser = new MyAppUser
            {
                Username = request.Username,
                Password = _authService.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
               
                IsCustomer = true,
                IsPharmacist = false,
                IsAdmin = false
            };

            _context.MyAppUsers.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = newUser.ID,
                username = newUser.Username,
                firstName = newUser.FirstName,
                lastName = newUser.LastName
            });
        }
    }
}
