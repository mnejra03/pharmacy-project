using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetChatEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetChatEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("user-conversations")]
        [Authorize]
        public async Task<ActionResult<List<ChatGetDTO>>> GetUserConversations()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            var userId = authInfo.UserId;

            var allMessages = await _context.Chats
                .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .OrderBy(c => c.Date)
                .Select(c => new ChatGetDTO
                {
                    Id = c.Id,
                    SenderId = c.SenderId,
                    SenderName = c.Sender.FirstName + " " + c.Sender.LastName,
                    ReceiverId = c.ReceiverId ?? 0,
                    ReceiverName = c.Receiver != null ? c.Receiver.FirstName + " " + c.Receiver.LastName : "Sistem",
                    Message = c.Message,
                    Date = c.Date,
                    TypeOfMessage = c.TypeOfMessage,
                    Status = c.Status,
                    IsResponse = c.IsResponse
                })
                .ToListAsync();

            return Ok(allMessages);
        }
    }
}
