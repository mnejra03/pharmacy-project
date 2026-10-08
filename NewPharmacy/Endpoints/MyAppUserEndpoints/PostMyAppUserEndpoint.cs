using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/users")]
    [ApiController]
    public class PostMyAppUserEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public PostMyAppUserEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> PostMyAppUser([FromBody] CreateMyAppUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("The App User must have a username and password.");
            }

            var existingUser = await _context.MyAppUsers
                .AnyAsync(x => x.Username == dto.Username);

            if (existingUser)
            {
                return BadRequest("Username already exists.");
            }

            var myAppUser = new MyAppUser
            {
                Username = dto.Username,
                Password = _authService.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                IsAdmin = dto.IsAdmin,
                IsPharmacist = dto.IsPharmacist,
                IsCustomer = dto.IsCustomer,
                IsDeleted = false
            };

            _context.MyAppUsers.Add(myAppUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                actionName: nameof(GetMyAppUserByIdEndpoint.GetMyAppUserById),
                controllerName: nameof(GetMyAppUserByIdEndpoint).Replace("Endpoint", string.Empty),
                routeValues: new { id = myAppUser.ID },
                value: new
            {
                myAppUser.ID,
                myAppUser.Username,
                myAppUser.FirstName,
                myAppUser.LastName,
                myAppUser.PhoneNumber,
                myAppUser.Email,
                myAppUser.IsAdmin,
                myAppUser.IsPharmacist,
                myAppUser.IsCustomer
            });
        }
    }
}
