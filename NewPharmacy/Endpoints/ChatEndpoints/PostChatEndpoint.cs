using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;
using NewPharmacy.SignalR;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly MyAuthService _authService;

        public ChatController(ApplicationDbContext context, IHubContext<ChatHub> hubContext, MyAuthService authService)
        {
            _context = context;
            _hubContext = hubContext;
            _authService = authService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ChatCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            if (!authInfo.IsCustomer && !authInfo.IsPharmacist)
            {
                return Forbid();
            }

            var sender = await _context.MyAppUsers.FindAsync(authInfo.UserId);
            if (sender == null)
            {
                return NotFound("Authenticated sender was not found.");
            }

            var receiver = await _context.MyAppUsers.FindAsync(dto.ReceiverId);
            if (receiver == null)
            {
                return NotFound("Receiver korisnik nije pronadjen.");
            }

            if (sender.IsCustomer && !receiver.IsPharmacist)
            {
                return BadRequest("Customers can send messages only to pharmacists.");
            }

            if (sender.IsPharmacist && !receiver.IsCustomer)
            {
                return BadRequest("Pharmacists can reply only to customers.");
            }

            var isResponse = sender.IsPharmacist;
            var typeOfMessage = sender.IsPharmacist ? "response" : "question";

            var chat = new Chat
            {
                SenderId = sender.ID,
                ReceiverId = receiver.ID,
                Message = dto.Message,
                Date = DateTime.UtcNow,
                TypeOfMessage = typeOfMessage,
                Status = dto.Status,
                IsResponse = isResponse
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            if (!isResponse)
            {
                var pharmacists = await _context.MyAppUsers
                    .Where(u => u.IsPharmacist)
                    .ToListAsync();

                foreach (var pharmacist in pharmacists)
                {
                    var notification = new Notification
                    {
                        Title = "Nova poruka od korisnika",
                        Message = $"Nova poruka od korisnika {sender.FirstName} {sender.LastName}: \"{dto.Message}\"",
                        MyAppUserId = pharmacist.ID,
                        Time = DateTime.UtcNow,
                        Type = "new_message",
                        SenderId = sender.ID,
                        Read = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    await _hubContext.Clients
                        .Group(pharmacist.ID.ToString())
                        .SendAsync("ReceiveNotification", new
                        {
                            id = notification.Id,
                            title = notification.Title,
                            message = notification.Message,
                            senderId = notification.SenderId,
                            time = notification.Time,
                            type = notification.Type
                        });
                }
            }

            await _hubContext.Clients
                .Group(receiver.ID.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    senderId = sender.ID,
                    message = dto.Message,
                    date = DateTime.UtcNow
                });

            return Ok();
        }
    }
}
