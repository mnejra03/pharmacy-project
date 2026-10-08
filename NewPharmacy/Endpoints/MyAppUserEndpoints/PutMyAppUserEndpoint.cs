using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/users")]
    [ApiController]
    public class PutMyAppUserEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public PutMyAppUserEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPut("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMyAppUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.ID)
            {
                return BadRequest("ID u URL-u i tijelu zahtjeva se ne podudaraju.");
            }

            var user = _context.MyAppUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = dto.Username;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            var rolesChanged =
                user.IsAdmin != dto.IsAdmin ||
                user.IsPharmacist != dto.IsPharmacist ||
                user.IsCustomer != dto.IsCustomer;
            user.IsAdmin = dto.IsAdmin;
            user.IsPharmacist = dto.IsPharmacist;
            user.IsCustomer = dto.IsCustomer;

            await _context.SaveChangesAsync();

            if (rolesChanged)
            {
                await _authService.InvalidateUserTokens(user.ID);
            }

            return Ok(user);
        }
    }
}
