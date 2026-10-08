namespace Market.Application.Abstractions;
public sealed record StoredFile(string Key, string RelativeUrl, string ContentType);
public interface IFileStorage
{
    Task<StoredFile> SaveAsync(byte[] content, string contentType, CancellationToken ct);
    Task<(byte[] Content, string ContentType)> ReadAsync(string key, CancellationToken ct);
}
