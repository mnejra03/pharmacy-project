namespace Market.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var hasher = new PasswordHasher<MarketUserEntity>();
        context.Users.AddRange(
            new MarketUserEntity
            {
                Email = "admin@pharmacy.local",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = hasher.HashPassword(null!, "Admin123!"),
                IsAdmin = true,
                IsCustomer = false,
                IsEnabled = true
            },
            new MarketUserEntity
            {
                Email = "pharmacist@pharmacy.local",
                FirstName = "Demo",
                LastName = "Pharmacist",
                PasswordHash = hasher.HashPassword(null!, "Pharmacist123!"),
                IsPharmacist = true,
                IsCustomer = false,
                IsEnabled = true
            }
        );
        await context.SaveChangesAsync();
    }
}
