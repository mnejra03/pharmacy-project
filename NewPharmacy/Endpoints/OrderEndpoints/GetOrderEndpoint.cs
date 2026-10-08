using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints
{
    [Route("api/orders")]
    [ApiController]
    public class GetOrderEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetOrderEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || (!authInfo.IsAdmin && !authInfo.IsPharmacist))
                return Forbid();

            return await _context.Orders
                .Include(n => n.MyAppUser)
                .Where(n => !n.IsDeleted)
                .ToListAsync();
        }
    }
}
