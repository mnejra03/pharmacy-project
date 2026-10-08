using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;
using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Endpoints.PharmacistEndpoints
{
    [Route("api/PharmacistDashboardEndpoint")]
    [ApiController]
    public class ChangePharmacistPasswordEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;
        private readonly ILogger<ChangePharmacistPasswordEndpoint> _logger;

        public ChangePharmacistPasswordEndpoint(
            ApplicationDbContext context,
            MyAuthService authService,
            ILogger<ChangePharmacistPasswordEndpoint> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("change-password")]
        [MyAuthorization(isAdmin: false, isPharmacist: true, isCustomer: false)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn || !authInfo.IsPharmacist)
            {
                return Unauthorized("Niste autentifikovani.");
            }

            var user = await _context.MyAppUsers
                .FirstOrDefaultAsync(x => x.ID == authInfo.UserId && x.IsPharmacist && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Password change failed because pharmacist was not found. UserId: {UserId}", authInfo.UserId);
                return NotFound("Korisnik nije pronađen.");
            }

            if (!_authService.VerifyPassword(dto.OldPassword, user.Password))
            {
                return BadRequest("Stara lozinka nije tačna.");
            }

            if (dto.OldPassword == dto.NewPassword)
            {
                return BadRequest("Nova lozinka mora biti različita od stare.");
            }

            user.Password = _authService.HashPassword(dto.NewPassword);

            try
            {
                await _context.SaveChangesAsync();
                await _authService.InvalidateUserTokens(user.ID);

                return Ok(new { Message = "Lozinka uspješno promijenjena." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving new password for pharmacist {UserId}.", authInfo.UserId);
                return StatusCode(500, "Greška pri spremanju nove lozinke.");
            }
        }
    }

    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
