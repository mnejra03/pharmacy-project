using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

public class AzureBlobService
{
    private readonly string _storageConnectionString;

    public AzureBlobService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration["AzureBlobStorage:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString) ||
            connectionString.Equals("unesite_pravi_connection_string", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage is not configured. Add a valid ConnectionStrings:AzureBlobStorage value to appsettings.Local.json.");
        }

        _storageConnectionString = connectionString;
    }

  
    public async Task<string> UploadImageAsync(IFormFile file, string containerName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

      
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // Existing containers keep their current permissions when CreateIfNotExists
        // is called, so explicitly allow anonymous reads for app image URLs.
        await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType
                }
            });
        }

        return blobClient.Uri.ToString();
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> DownloadImageAsync(string imageUrl)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Invalid image URL.");

        var serviceClient = new BlobServiceClient(_storageConnectionString);
        if (!string.Equals(uri.Host, serviceClient.Uri.Host, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Image is not stored in the configured blob account.");

        var containerName = uri.AbsolutePath.Trim('/').Split('/', 2)[0];
        var blobName = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/').Split('/', 2).ElementAtOrDefault(1) ?? string.Empty);
        if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Invalid image URL.");

        var blobClient = serviceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);
        var download = await blobClient.DownloadContentAsync();
        return (download.Value.Content.ToArray(), download.Value.Details.ContentType ?? "application/octet-stream", Path.GetFileName(blobName));
    }
}
