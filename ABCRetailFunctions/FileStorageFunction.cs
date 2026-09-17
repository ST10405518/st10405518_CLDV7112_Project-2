using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure;

public class FileStorageFunction
{
    private readonly ILogger<FileStorageFunction> _logger;
    private readonly string _connectionString;

    public FileStorageFunction(ILogger<FileStorageFunction> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureStorage__ConnectionString") ?? "UseDevelopmentStorage=true";
    }

    [Function("UploadLogFile")]
    public async Task<HttpResponseData> UploadLogFile(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "file/upload")] HttpRequestData req)
    {
        _logger.LogInformation("Processing log file upload");

        try
        {
            var uploadRequest = await req.ReadFromJsonAsync<FileUploadRequest>();
            
            if (uploadRequest == null || string.IsNullOrEmpty(uploadRequest.FileName) || string.IsNullOrEmpty(uploadRequest.Content))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid file upload data");
                return badResponse;
            }

            var shareServiceClient = new ShareServiceClient(_connectionString);
            var shareClient = shareServiceClient.GetShareClient("log-files");
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(uploadRequest.FileName);

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(uploadRequest.Content));
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Log file uploaded successfully: {uploadRequest.FileName}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading log file: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("CreateLogEntry")]
    public async Task<HttpResponseData> CreateLogEntry(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "file/log")] HttpRequestData req)
    {
        _logger.LogInformation("Creating log entry");

        try
        {
            var logRequest = await req.ReadFromJsonAsync<LogEntryRequest>();
            
            if (logRequest == null || string.IsNullOrEmpty(logRequest.Content))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid log entry data");
                return badResponse;
            }

            var shareServiceClient = new ShareServiceClient(_connectionString);
            var shareClient = shareServiceClient.GetShareClient("log-files");
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileName = $"log_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
            var fileClient = directoryClient.GetFileClient(fileName);

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(logRequest.Content));
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Log entry created successfully: {fileName}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating log entry: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetLogFiles")]
    public async Task<HttpResponseData> GetLogFiles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "file/list")] HttpRequestData req)
    {
        _logger.LogInformation("Retrieving all log files");

        try
        {
            var shareServiceClient = new ShareServiceClient(_connectionString);
            var shareClient = shareServiceClient.GetShareClient("log-files");
            var directoryClient = shareClient.GetRootDirectoryClient();

            var files = new List<ShareFileItem>();
            await foreach (var file in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!file.IsDirectory)
                {
                    files.Add(file);
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(files);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving log files: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("DownloadLogFile")]
    public async Task<HttpResponseData> DownloadLogFile(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "file/{fileName}")] HttpRequestData req,
        string fileName)
    {
        _logger.LogInformation($"Downloading log file: {fileName}");

        try
        {
            var shareServiceClient = new ShareServiceClient(_connectionString);
            var shareClient = shareServiceClient.GetShareClient("log-files");
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            var exists = await fileClient.ExistsAsync();
            if (!exists.Value)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"File not found: {fileName}");
                return notFoundResponse;
            }

            var download = await fileClient.DownloadAsync();
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain");
            response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            
            await download.Value.Content.CopyToAsync(response.Body);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error downloading log file: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("DeleteLogFile")]
    public async Task<HttpResponseData> DeleteLogFile(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "file/{fileName}")] HttpRequestData req,
        string fileName)
    {
        _logger.LogInformation($"Deleting log file: {fileName}");

        try
        {
            var shareServiceClient = new ShareServiceClient(_connectionString);
            var shareClient = shareServiceClient.GetShareClient("log-files");
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            await fileClient.DeleteIfExistsAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Log file deleted successfully: {fileName}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting log file: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }
}

public class LogEntryRequest
{
    public string Content { get; set; } = string.Empty;
}

public class FileUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
