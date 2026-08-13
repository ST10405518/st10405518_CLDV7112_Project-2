using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetailApp.Services;

public class LocalFileItem
{
    public string Name { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class FileStorageService
{
    private readonly ShareClient? _shareClient;
    private readonly ShareDirectoryClient? _logDirectoryClient;
    private readonly string _connectionString;
    private readonly bool _useLocalFallback;
    private readonly string _localLogPath;

    public FileStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"]!;
        _localLogPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalLogFiles");
        
        try
        {
            var shareServiceClient = new ShareServiceClient(_connectionString);
            _shareClient = shareServiceClient.GetShareClient("log-files");
            _shareClient.CreateIfNotExists();
            
            _logDirectoryClient = _shareClient.GetRootDirectoryClient();
            _useLocalFallback = false;
        }
        catch (Exception ex)
        {
            // Azure Files doesn't support development emulator
            // Use local file system as fallback
            Console.WriteLine($"Azure Files not available in emulator, using local file system: {ex.Message}");
            _useLocalFallback = true;
            
            // Create local directory if it doesn't exist
            if (!Directory.Exists(_localLogPath))
            {
                Directory.CreateDirectory(_localLogPath);
            }
        }
    }

    public async Task UploadLogFileAsync(string fileName, Stream content)
    {
        if (_useLocalFallback)
        {
            var filePath = Path.Combine(_localLogPath, fileName);
            using var fileStream = File.Create(filePath);
            await content.CopyToAsync(fileStream);
        }
        else
        {
            var fileClient = _logDirectoryClient!.GetFileClient(fileName);
            await fileClient.CreateAsync(content.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, content.Length), content);
        }
    }

    public async Task<Stream?> DownloadLogFileAsync(string fileName)
    {
        if (_useLocalFallback)
        {
            var filePath = Path.Combine(_localLogPath, fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.OpenRead(filePath);
        }
        else
        {
            var fileClient = _logDirectoryClient!.GetFileClient(fileName);
            
            if (!await fileClient.ExistsAsync())
            {
                return null;
            }
            
            var download = await fileClient.DownloadAsync();
            return download.Value.Content;
        }
    }

    public async Task<List<object>> GetAllLogFilesAsync()
    {
        if (_useLocalFallback)
        {
            // Return local files as LocalFileItem objects
            var files = Directory.GetFiles(_localLogPath);
            var result = new List<object>();
            
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                result.Add(new LocalFileItem
                {
                    Name = fileInfo.Name,
                    FileSize = fileInfo.Length
                });
            }
            
            return result;
        }
        else
        {
            var files = new List<object>();
            await foreach (var file in _logDirectoryClient!.GetFilesAndDirectoriesAsync())
            {
                if (!file.IsDirectory)
                {
                    files.Add(file);
                }
            }
            return files;
        }
    }

    public async Task DeleteLogFileAsync(string fileName)
    {
        if (_useLocalFallback)
        {
            var filePath = Path.Combine(_localLogPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        else
        {
            var fileClient = _logDirectoryClient!.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
    }

    public async Task<bool> LogFileExistsAsync(string fileName)
    {
        if (_useLocalFallback)
        {
            var filePath = Path.Combine(_localLogPath, fileName);
            return File.Exists(filePath);
        }
        else
        {
            var fileClient = _logDirectoryClient!.GetFileClient(fileName);
            var response = await fileClient.ExistsAsync();
            return response.Value;
        }
    }

    public async Task<string> CreateLogEntryAsync(string logContent)
    {
        var fileName = $"log_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(logContent));
        await UploadLogFileAsync(fileName, stream);
        return fileName;
    }
}
