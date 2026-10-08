using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints.NotificationEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutNotificationEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public PutNotificationEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.MyAppUserId == authInfo.UserId);

            if (notification == null)
                return NotFound();

            notification.Read = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
