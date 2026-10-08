using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserChatHistoryEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetUserChatHistoryEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserChatHistory()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var userId = authInfo.UserId;

            var chats = await _context.Chats
                .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                .OrderByDescending(c => c.Date)
                .Select(c => new
                {
                    c.Id,
                    c.SenderId,
                    c.ReceiverId,
                    c.Message,
                    c.Date,
                    c.IsResponse,
                    c.TypeOfMessage
                })
                .ToListAsync();

            return Ok(chats);
        }

        [HttpGet("last-pharmacist")]
        [Authorize]
        public async Task<IActionResult> GetLastPharmacist()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var userId = authInfo.UserId;

            var lastPharmacistResponse = await _context.Chats
                .Where(c => c.ReceiverId == userId && c.IsResponse == true)
                .OrderByDescending(c => c.Date)
                .Select(c => new
                {
                    PharmacistId = c.SenderId,
                    LastMessageDate = c.Date
                })
                .FirstOrDefaultAsync();

            return Ok(lastPharmacistResponse);
        }
    }
}
