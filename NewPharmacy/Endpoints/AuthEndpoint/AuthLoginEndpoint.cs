using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;
using System.Threading;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.Auth
{
    [Route("auth")]
    [ApiController]
    public class AuthLoginEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly MyAuthService _authService;

        public AuthLoginEndpoint(ApplicationDbContext db, MyAuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _db.MyAppUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return Unauthorized(new { Message = "Incorrect username or password" });
            }

            var passwordMatchesHash = _authService.VerifyPassword(request.Password, user.Password);
            var passwordMatchesLegacyValue = !passwordMatchesHash &&
                string.Equals(user.Password, request.Password, StringComparison.Ordinal);

            if (!passwordMatchesHash && !passwordMatchesLegacyValue)
            {
                return Unauthorized(new { Message = "Incorrect username or password" });
            }

            // Older database rows stored passwords as plain text. Upgrade a matching
            // legacy value to the current salted hash format during the successful login.
            if (passwordMatchesLegacyValue)
            {
                user.Password = _authService.HashPassword(request.Password);
                await _db.SaveChangesAsync(cancellationToken);
            }

            var authToken = await _authService.GenerateAuthToken(user, cancellationToken);
            var authInfo = _authService.CreateAuthInfo(user);

            return Ok(new LoginResponse
            {
                Token = authToken.Value,
                MyAuthInfo = authInfo
            });
        }

        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class LoginResponse
        {
            public required MyAuthInfo MyAuthInfo { get; set; }
            public required string Token { get; set; }
        }
    }
}
