using System.Security.Claims;

namespace NewPharmacy.Data.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLoggedInUsername(this ClaimsPrincipal user)
        {
            return user?.Identity?.Name ?? throw new Exception("Nema prijavljenog korisnika.");
        }
    }

}
