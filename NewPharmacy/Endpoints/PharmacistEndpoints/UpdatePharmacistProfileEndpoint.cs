using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.PharmacistEndpoints
{
    [ApiController]
    [Route("api/PharmacistDashboardEndpoint")]
    public class UpdatePharmacistProfileEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePharmacistProfileEndpoint> _logger;
        private readonly MyAuthService _authService;
        private readonly AzureBlobService _blobService;

        public UpdatePharmacistProfileEndpoint(
            ApplicationDbContext context,
            ILogger<UpdatePharmacistProfileEndpoint> logger,
            MyAuthService authService,
            AzureBlobService blobService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
            _blobService = blobService;
        }

        [HttpPut("update-profile")]
        [MyAuthorization(isAdmin: false, isPharmacist: true, isCustomer: false)]
        public async Task<IActionResult> UpdateProfile(
            [FromForm] string email,
            [FromForm] IFormFile? profileImage)
        {
            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn || !authInfo.IsPharmacist)
            {
                return Unauthorized();
            }

            var user = await _context.MyAppUsers
                .FirstOrDefaultAsync(x => x.ID == authInfo.UserId && x.IsPharmacist && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Pharmacist profile update failed because the user was not found. UserId: {UserId}", authInfo.UserId);
                return NotFound("Pharmacist not found.");
            }

            user.Email = email;

            if (profileImage != null)
            {
                if (!profileImage.ContentType.StartsWith("image/"))
                    return BadRequest("Only image files are allowed.");

                if (profileImage.Length > 5 * 1024 * 1024)
                    return BadRequest("Image size must be under 5MB.");

                user.ProfileImageUrl = await _blobService.UploadImageAsync(profileImage, "profile-images");
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                user.Email,
                user.ProfileImageUrl
            });
        }
    }
}
