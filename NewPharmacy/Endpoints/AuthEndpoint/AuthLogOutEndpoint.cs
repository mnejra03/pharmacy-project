using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Services;
using System.Threading;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.Auth
{
    [Route("auth")]
    [ApiController]
    public class AuthLogoutEndpoint : ControllerBase
    {
        private readonly MyAuthService _authService;

        public AuthLogoutEndpoint(MyAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken = default)
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            var token = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader["Bearer ".Length..].Trim()
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(token))
            {
                await _authService.InvalidateToken(token, cancellationToken);
            }

            return Ok(new { Message = "Successfully logged out" });
        }
    }
}
