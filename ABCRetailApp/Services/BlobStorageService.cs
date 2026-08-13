using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetailApp.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _connectionString;

    public BlobStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"]!;
        
        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient("product-images");
            _containerClient.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            // Log the error but allow the application to start
            Console.WriteLine($"Azure Storage Emulator not available: {ex.Message}");
            var blobServiceClient = new BlobServiceClient(_connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient("product-images");
        }
    }

    public async Task UploadBlobAsync(string blobName, Stream content, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = contentType
        };
        
        await blobClient.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });
    }

    public async Task<Stream?> DownloadBlobAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        
        if (!await blobClient.ExistsAsync())
        {
            return null;
        }
        
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    public async Task<List<BlobItem>> GetAllBlobsAsync()
    {
        var blobs = new List<BlobItem>();
        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            blobs.Add(blob);
        }
        return blobs;
    }

    public async Task<string> GetBlobUrlAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<bool> BlobExistsAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.ExistsAsync();
        return response.Value;
    }
}
