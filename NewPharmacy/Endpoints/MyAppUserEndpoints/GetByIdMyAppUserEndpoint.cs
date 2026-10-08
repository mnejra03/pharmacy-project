using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;

namespace NewPharmacy.Endpoints
{
    [Route("api/users")]
    [ApiController]
    public class GetMyAppUserByIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetMyAppUserByIdEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public IActionResult GetMyAppUserById(int id)
        {
            var myAppUser = _context.MyAppUsers
                .FirstOrDefault(u => u.ID == id && !u.IsDeleted);

            if (myAppUser == null)
                return NotFound($"App user with Id {id} not found.");

            return Ok(myAppUser);
        }
    }
}
