using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data.Models;
using NewPharmacy.Data.Models.Auth;

namespace NewPharmacy.Data
{
    public class ApplicationDbContext(
        DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListDetail> WishListDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<MyAppUser> MyAppUsers { get; set; }
        public DbSet<MyAuthenticationToken> MyAuthenticationTokens { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            modelBuilder.Entity<Product>()
            .Property(p => p.DatumDodavanja)
            .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<WishList>()
                .HasIndex(w => w.MyAppUserId)
                .IsUnique();

            modelBuilder.Entity<WishListDetail>()
                .HasIndex(wd => new { wd.WishListId, wd.ProductId })
                .IsUnique();

            
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Product>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdateDiscountedPrice();
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
