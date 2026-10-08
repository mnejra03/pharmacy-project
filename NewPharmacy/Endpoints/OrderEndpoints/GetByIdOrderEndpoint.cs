using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints
{
    [Route("api/orders")]
    [ApiController]
    public class GetOrderByIdEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetOrderByIdEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || (!authInfo.IsAdmin && !authInfo.IsPharmacist))
                return Forbid();

            var order = await _context.Orders
                .Include(n => n.MyAppUser)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (order == null)
                return NotFound("Order with Id {id} not found.");

            return Ok(order);
        }
    }
}
