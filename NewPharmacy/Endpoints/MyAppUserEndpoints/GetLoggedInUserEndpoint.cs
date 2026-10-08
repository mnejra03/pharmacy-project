using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace NewPharmacy.Endpoints.MyAppUserEndpoints
{
    public class GetLoggedInUserEndpoint : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetLoggedInUser()
        {
            var user = HttpContext.GetLoggedInUser(); 
            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
    }
}
