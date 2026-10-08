using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistDashboardEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PharmacistDashboardEndpoint> _logger;
        private readonly MyAuthService _authService;

        public PharmacistDashboardEndpoint(
            ApplicationDbContext context,
            ILogger<PharmacistDashboardEndpoint> logger,
            MyAuthService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }

        [HttpGet("pharmacist-profile")]
        [MyAuthorization(isAdmin: false, isPharmacist: true, isCustomer: false)]
        public async Task<IActionResult> GetPharmacistProfile()
        {
            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn || !authInfo.IsPharmacist)
            {
                return Unauthorized(new { Message = "Korisnik nije autentificiran." });
            }

            var pharmacist = await _context.MyAppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == authInfo.UserId && x.IsPharmacist && !x.IsDeleted);

            if (pharmacist == null)
            {
                _logger.LogWarning("Pharmacist profile not found for authenticated user {UserId}.", authInfo.UserId);
                return NotFound(new { Message = "Farmaceut nije pronađen" });
            }

            return Ok(new
            {
                pharmacist.Username,
                pharmacist.FirstName,
                pharmacist.LastName,
                pharmacist.Email,
                pharmacist.EmploymentDate,
                pharmacist.ProfileImageUrl
            });
        }
    }
}
