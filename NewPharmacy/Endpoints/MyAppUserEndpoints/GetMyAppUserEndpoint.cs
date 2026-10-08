using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Services;

namespace NewPharmacy.Endpoints
{
    [Route("api/users")]
    [ApiController]
    public class GetMyAppUserEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public GetMyAppUserEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public IActionResult GetMyAppUser()
        {
            var myAppUsers = _context.MyAppUsers
                .Where(u => !u.IsDeleted)
                .ToList();
            return Ok(myAppUsers);
        }

        [HttpGet("paged")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public IActionResult GetMyAppUserPaged(
            [FromQuery] string? username,
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.MyAppUsers
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.Username.ToLower().Contains(username.ToLower()));

            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(u => u.FirstName.ToLower().Contains(firstName.ToLower()));

            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(u => u.LastName.ToLower().Contains(lastName.ToLower()));

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (role == "isAdmin") query = query.Where(u => u.IsAdmin);
                if (role == "isPharmacist") query = query.Where(u => u.IsPharmacist);
                if (role == "isCustomer") query = query.Where(u => u.IsCustomer);
            }

            var totalCount = query.Count();

            var items = query
                .OrderBy(u => u.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    id = u.ID,
                    username = u.Username,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    isAdmin = u.IsAdmin,
                    isPharmacist = u.IsPharmacist,
                    isCustomer = u.IsCustomer
                })
                .ToList();

            return Ok(new
            {
                items,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("pharmacist")]
        [MyAuthorization(isAdmin: true, isPharmacist: true, isCustomer: true)]
        public IActionResult GetPharmacistData()
        {
            var pharmacists = _context.MyAppUsers
                .AsNoTracking()
                .Where(u => u.IsPharmacist && !u.IsDeleted)
                .Select(u => new
                {
                    u.ID,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.ProfileImageUrl
                })
                .ToList();

            if (pharmacists.Count == 0)
                return NotFound(new { Message = "Nema farmaceuta u bazi podataka." });

            return Ok(pharmacists);
        }

        [HttpGet("pharmacist/{id}")]
        [MyAuthorization(isAdmin: true, isPharmacist: true, isCustomer: false)]
        public IActionResult GetPharmacistWithDashboard(int id)
        {
            var pharmacist = _context.MyAppUsers
                .AsNoTracking()
                .Where(u => u.IsPharmacist && !u.IsDeleted && u.ID == id)
                .Select(u => new
                {
                    u.ID,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.EmploymentDate,
                    u.ProfileImageUrl
                })
                .FirstOrDefault();

            if (pharmacist == null)
                return NotFound(new { Message = "Farmaceut nije pronađen." });

            var today = DateTime.Today;
            var totalMedications = _context.Products.Count();
            var expiringSoon = _context.Products.Count(p => p.ExpiryDate <= today.AddDays(30));
            var lowStock = _context.Products.Count(p => p.QuantityInStock < 10);

            return Ok(new
            {
                Pharmacist = pharmacist,
                Dashboard = new
                {
                    TotalMedications = totalMedications,
                    ExpiringSoon = expiringSoon,
                    LowStock = lowStock
                }
            });
        }

        [HttpGet("pharmacist/dashboard")]
        [MyAuthorization(isAdmin: false, isPharmacist: true, isCustomer: false)]
        public IActionResult GetLoggedInPharmacistProfile()
        {
            var authInfo = _authService.GetAuthInfo();
            if (!authInfo.IsLoggedIn)
                return Unauthorized(new { message = "Korisnik nije autentificiran." });

            var pharmacist = _context.MyAppUsers
                .AsNoTracking()
                .FirstOrDefault(x => x.ID == authInfo.UserId && x.IsPharmacist && !x.IsDeleted);

            if (pharmacist == null)
                return NotFound(new { message = "Farmaceut nije pronađen." });

            return Ok(new
            {
                pharmacist.FirstName,
                pharmacist.LastName,
                pharmacist.Username,
                pharmacist.Email,
                pharmacist.ProfileImageUrl,
                pharmacist.EmploymentDate
            });
        }
    }
}
