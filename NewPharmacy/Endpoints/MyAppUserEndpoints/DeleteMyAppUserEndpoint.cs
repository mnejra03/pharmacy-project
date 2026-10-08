using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/users")]
    [ApiController]
    public class DeleteMyAppUserEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public DeleteMyAppUserEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpDelete("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<IActionResult> DeleteMyAppUser(int id)
        {
            var user = _context.MyAppUsers.FirstOrDefault(u => u.ID == id);

            if (user == null)
            {
                return NotFound($"User with id {id} not found.");
            }

            user.IsDeleted = true;

            await _context.SaveChangesAsync();
            await _authService.InvalidateUserTokens(user.ID);

            return NoContent();
        }
    }
}
