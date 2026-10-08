using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Helper.Api;
using NewPharmacy.Services;
using System.Threading;
using System.Threading.Tasks;
using static NewPharmacy.Endpoints.AuthEndpoints.AuthGetEndpoint;

namespace NewPharmacy.Endpoints.AuthEndpoints
{
    [Route("auth")]
    public class AuthGetEndpoint(MyAuthService authService) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<AuthGetResponse>
    {
        [HttpGet]
        [Authorize]
        public override async Task<ActionResult<AuthGetResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var authInfo = authService.GetAuthInfo();

            if (!authInfo.IsLoggedIn)
            {
                return Unauthorized("Invalid or expired token");
            }

            return Ok(new AuthGetResponse
            {
                MyAuthInfo = authInfo
            });
        }

        public class AuthGetResponse
        {
            public required MyAuthInfo MyAuthInfo { get; set; }
        }
    }
}
