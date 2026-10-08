using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteOrderEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public DeleteOrderEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || (!authInfo.IsAdmin && !authInfo.IsPharmacist))
                return Forbid();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (order == null)
                return NotFound("Narudzba nije pronadjena.");

            if (order.Status == "Delivered")
                return BadRequest("Isporucena narudzba se ne moze obrisati.");

            order.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
