using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewPharmacy.Helper
{
    public static class FileHelper
    {
        public static async Task<string> UploadImageAsync(byte[] imageBytes, string fileName)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                var physicalPath = Path.Combine(uploadsFolder, safeFileName);

                await File.WriteAllBytesAsync(physicalPath, imageBytes);

                return $"/uploads/{safeFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Greska pri cuvanju slike.", ex);
            }
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, iterations: 100_000, HashAlgorithmName.SHA256
            );

            byte[] hash = pbkdf2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] originalHash = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, iterations: 100_000, HashAlgorithmName.SHA256
            );

            byte[] inputHash = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(inputHash, originalHash);
        }

    }
}
