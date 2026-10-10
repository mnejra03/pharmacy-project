using System.Reflection;
using System.Text.Json;

namespace Market.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        var hasher = new PasswordHasher<MarketUserEntity>();
        if (!await context.Users.AnyAsync(u => u.Email == "admin@pharmacy.local"))
            context.Users.Add(new MarketUserEntity { Email = "admin@pharmacy.local", FirstName = "System", LastName = "Administrator", PasswordHash = hasher.HashPassword(null!, "Admin123!"), IsAdmin = true, IsCustomer = false, IsEnabled = true });
        if (!await context.Users.AnyAsync(u => u.Email == "pharmacist@pharmacy.local"))
            context.Users.Add(new MarketUserEntity { Email = "pharmacist@pharmacy.local", FirstName = "Demo", LastName = "Pharmacist", PasswordHash = hasher.HashPassword(null!, "Pharmacist123!"), IsPharmacist = true, IsCustomer = false, IsEnabled = true });
        await context.SaveChangesAsync();

        await SeedCatalogAsync(context);

        var advertisements = new[]
        {
            ("Yasenka skinage beauty", "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/superapoteka_web_novembar_Yasenka%20skinage%20beauty.jpg"),
            ("Yasenka shake", "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/superapoteka_web_novembar_Yasenka_shake.jpg"),
            ("Defendil", "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/DEFENDIL%201.jpg"),
            ("Waya", "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/WAYA%201.jpg"),
            ("Ducray set", "https://rs1pharmacyimages.blob.core.windows.net/advertisement-images/Ducray%20set%201.jpg")
        };
        foreach (var (title, imageUrl) in advertisements)
            if (!await context.Advertisements.AnyAsync(a => a.Title == title)) context.Advertisements.Add(new AdvertisementEntity { Title = title, ImageUrl = imageUrl });
        await context.SaveChangesAsync();
    }

    private static async Task SeedCatalogAsync(DatabaseContext context)
    {
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Market.Infrastructure.Database.Seeders.legacy-products.json")
            ?? throw new InvalidOperationException("Katalog proizvoda nije pronađen u resursima aplikacije.");
        var catalog = await JsonSerializer.DeserializeAsync<LegacyCatalog>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Katalog proizvoda nije ispravan.");

        foreach (var name in catalog.Categories)
            if (!await context.ProductCategories.AnyAsync(c => c.Name == name)) context.ProductCategories.Add(new ProductCategoryEntity { Name = name });
        foreach (var name in catalog.Brands)
            if (!await context.Brands.AnyAsync(b => b.Name == name)) context.Brands.Add(new BrandEntity { Name = name, Description = $"{name} proizvodi" });
        await context.SaveChangesAsync();

        var categories = await context.ProductCategories.ToDictionaryAsync(c => c.Name, c => c.Id);
        var brands = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);
        var existingProducts = await context.Products.ToDictionaryAsync(p => p.Name);
        foreach (var item in catalog.Products)
        {
            if (existingProducts.TryGetValue(item.Name, out var existing))
            {
                // Replace image links from the retired blob account while preserving
                // any image URL that an administrator has since uploaded or changed.
                if (existing.ImageUrl.StartsWith("https://rs1pharmacyimages.blob.core.windows.net/", StringComparison.OrdinalIgnoreCase)
                    || existing.ImageUrl.StartsWith("/images/products/", StringComparison.OrdinalIgnoreCase))
                    existing.ImageUrl = item.ImageUrl;
                continue;
            }
            var categoryName = catalog.Categories[item.CategoryIndex];
            var brandId = item.BrandIndex is int index && index >= 0 && index < catalog.Brands.Count ? brands[catalog.Brands[index]] : (int?)null;
            context.Products.Add(new ProductEntity
            {
                Name = item.Name, Description = item.Description, Price = item.Price, QuantityInStock = item.Quantity,
                ImageUrl = item.ImageUrl, CategoryId = categories[categoryName], BrandId = brandId,
                IsDiscounted = item.Discounted, DiscountPercentage = item.Discounted ? item.DiscountPercentage : null,
                ExpiryDate = item.ExpiryDate, AddedAtUtc = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();
    }

    private sealed class LegacyCatalog
    {
        public List<string> Categories { get; set; } = [];
        public List<string> Brands { get; set; } = [];
        public List<LegacyProduct> Products { get; set; } = [];
    }

    private sealed class LegacyProduct
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int CategoryIndex { get; set; }
        public int? BrandIndex { get; set; }
        public bool Discounted { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
