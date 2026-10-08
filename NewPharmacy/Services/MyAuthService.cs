using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NewPharmacy.Data;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewPharmacy.Services
{
    public class MyAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MyAuthService> _logger;

        public MyAuthService(
            ApplicationDbContext db,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<MyAuthService> logger)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        public string HashPassword(string password)
        {
            return FileHelper.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return FileHelper.VerifyPassword(password, hashedPassword);
        }

        public async Task<MyAuthenticationToken> GenerateAuthToken(MyAppUser user, CancellationToken cancellationToken = default)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "1440");

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.LastName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? ""),
                new Claim("Email", user.Email ?? ""),
                new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("IsPharmacist", user.IsPharmacist.ToString()),
                new Claim("IsCustomer", user.IsCustomer.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var authToken = new MyAuthenticationToken
            {
                Value = tokenString,
                MyAppUserId = user.ID,
                RecordedAt = DateTime.UtcNow,
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            };

            _db.MyAuthenticationTokens.Add(authToken);
            await _db.SaveChangesAsync(cancellationToken);

            return authToken;
        }

        public MyAuthInfo GetAuthInfo()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return new MyAuthInfo { IsLoggedIn = false };
            }

            var userId = GetUserIdFromPrincipal(principal);
            if (userId == null)
            {
                return new MyAuthInfo { IsLoggedIn = false };
            }

            var user = _db.MyAppUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.ID == userId.Value && !u.IsDeleted);

            if (user == null)
            {
                return new MyAuthInfo { IsLoggedIn = false };
            }

            return CreateAuthInfo(user);
        }

        public MyAuthInfo CreateAuthInfo(MyAppUser user)
        {
            return new MyAuthInfo
            {
                IsLoggedIn = true,
                UserId = user.ID,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                IsPharmacist = user.IsPharmacist,
                IsCustomer = user.IsCustomer
            };
        }

        public async Task<bool> InvalidateToken(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await _db.MyAuthenticationTokens
                    .FirstOrDefaultAsync(t => t.Value == token, cancellationToken);

                if (authToken != null)
                {
                    _db.MyAuthenticationTokens.Remove(authToken);
                    await _db.SaveChangesAsync(cancellationToken);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token invalidation failed.");
                return false;
            }
        }

        public async Task<int> InvalidateUserTokens(int userId, CancellationToken cancellationToken = default)
        {
            var userTokens = await _db.MyAuthenticationTokens
                .Where(t => t.MyAppUserId == userId)
                .ToListAsync(cancellationToken);

            if (userTokens.Count == 0)
            {
                return 0;
            }

            _db.MyAuthenticationTokens.RemoveRange(userTokens);
            await _db.SaveChangesAsync(cancellationToken);

            return userTokens.Count;
        }

        public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var authToken = await _db.MyAuthenticationTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Value == token, cancellationToken);

            if (authToken == null)
            {
                return false;
            }

            return await _db.MyAppUsers
                .AsNoTracking()
                .AnyAsync(u => u.ID == authToken.MyAppUserId && !u.IsDeleted, cancellationToken);
        }

        public bool HasRole(string role)
        {
            var authInfo = GetAuthInfo();

            return role.ToLower() switch
            {
                "admin" => authInfo.IsAdmin,
                "pharmacist" => authInfo.IsPharmacist,
                "customer" => authInfo.IsCustomer,
                _ => false
            };
        }

        public int? GetCurrentUserId()
        {
            var authInfo = GetAuthInfo();
            return authInfo.IsLoggedIn ? authInfo.UserId : null;
        }

        public async Task<MyAppUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return null;
            }

            return await _db.MyAppUsers
                .FirstOrDefaultAsync(u => u.ID == userId.Value && !u.IsDeleted, cancellationToken);
        }

        private static int? GetUserIdFromPrincipal(ClaimsPrincipal principal)
        {
            return int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                ? userId
                : null;
        }
    }

    public class MyAuthInfo
    {
        public bool IsLoggedIn { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPharmacist { get; set; }
        public bool IsCustomer { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
