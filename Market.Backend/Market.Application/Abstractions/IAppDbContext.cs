namespace Market.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<MarketUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }
    DbSet<ChatMessageEntity> ChatMessages { get; }
    DbSet<NotificationEntity> Notifications { get; }
    DbSet<RecipeEntity> Recipes { get; }
    DbSet<AdvertisementEntity> Advertisements { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}
