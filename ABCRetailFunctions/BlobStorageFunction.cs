using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobUploadRequest
{
    public string BlobName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class BlobStorageFunction
{
    private readonly ILogger<BlobStorageFunction> _logger;
    private readonly string _connectionString;

    public BlobStorageFunction(ILogger<BlobStorageFunction> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureStorage__ConnectionString") ?? "UseDevelopmentStorage=true";
    }

    [Function("UploadBlob")]
    public async Task<HttpResponseData> UploadBlob(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blob/upload")] HttpRequestData req)
    {
        _logger.LogInformation("Processing blob upload request");

        try
        {
            var uploadRequest = await req.ReadFromJsonAsync<BlobUploadRequest>();
            
            if (uploadRequest == null || string.IsNullOrEmpty(uploadRequest.BlobName) || uploadRequest.Content == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid blob upload data");
                return badResponse;
            }

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("product-images");
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(uploadRequest.BlobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = uploadRequest.ContentType ?? "application/octet-stream"
            };

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(uploadRequest.Content));
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Blob uploaded successfully: {uploadRequest.BlobName}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading blob: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetBlobs")]
    public async Task<HttpResponseData> GetBlobs(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blob/list")] HttpRequestData req)
    {
        _logger.LogInformation("Retrieving all blobs");

        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("product-images");

            var blobs = new List<BlobItem>();
            await foreach (var blob in containerClient.GetBlobsAsync())
            {
                blobs.Add(blob);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(blobs);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving blobs: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("DeleteBlob")]
    public async Task<HttpResponseData> DeleteBlob(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "blob/{blobName}")] HttpRequestData req,
        string blobName)
    {
        _logger.LogInformation($"Deleting blob: {blobName}");

        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("product-images");
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Blob deleted successfully: {blobName}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting blob: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetBlobUrl")]
    public async Task<HttpResponseData> GetBlobUrl(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blob/{blobName}/url")] HttpRequestData req,
        string blobName)
    {
        _logger.LogInformation($"Getting blob URL: {blobName}");

        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("product-images");
            var blobClient = containerClient.GetBlobClient(blobName);

            var exists = await blobClient.ExistsAsync();
            if (!exists.Value)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Blob not found: {blobName}");
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { url = blobClient.Uri.ToString() });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting blob URL: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }
}
