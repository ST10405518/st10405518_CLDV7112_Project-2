using ABCRetailApp.Services;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ABCRetailApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly TableStorageService _tableStorageService;
    private readonly BlobStorageService _blobStorageService;
    private readonly QueueStorageService _queueStorageService;
    private readonly FileStorageService _fileStorageService;

    public List<CustomerEntity> Customers { get; set; } = new();
    public List<ProductEntity> Products { get; set; } = new();
    public List<BlobItem> Blobs { get; set; } = new();
    public List<object> LogFiles { get; set; } = new();
    public List<PeekedMessage> OrderQueueMessages { get; set; } = new();
    public List<PeekedMessage> InventoryQueueMessages { get; set; } = new();
    public int OrderQueueLength { get; set; }
    public int InventoryQueueLength { get; set; }
    public string? ActiveTab { get; set; }

    public IndexModel(
        ILogger<IndexModel> logger,
        TableStorageService tableStorageService,
        BlobStorageService blobStorageService,
        QueueStorageService queueStorageService,
        FileStorageService fileStorageService)
    {
        _logger = logger;
        _tableStorageService = tableStorageService;
        _blobStorageService = blobStorageService;
        _queueStorageService = queueStorageService;
        _fileStorageService = fileStorageService;
    }

    public async Task OnGetAsync(string? activeTab = null)
    {
        ActiveTab = activeTab;
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Customers = await _tableStorageService.GetAllCustomersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load customers from Table Storage");
            Customers = new List<CustomerEntity>();
        }

        try
        {
            Products = await _tableStorageService.GetAllProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load products from Table Storage");
            Products = new List<ProductEntity>();
        }

        try
        {
            Blobs = await _blobStorageService.GetAllBlobsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load blobs from Blob Storage");
            Blobs = new List<BlobItem>();
        }

        try
        {
            LogFiles = await _fileStorageService.GetAllLogFilesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load log files from File Storage");
            LogFiles = new List<object>();
        }

        try
        {
            OrderQueueMessages = await _queueStorageService.PeekOrderQueueMessagesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to peek order queue messages");
            OrderQueueMessages = new List<PeekedMessage>();
        }

        try
        {
            InventoryQueueMessages = await _queueStorageService.PeekInventoryQueueMessagesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to peek inventory queue messages");
            InventoryQueueMessages = new List<PeekedMessage>();
        }

        try
        {
            OrderQueueLength = await _queueStorageService.GetOrderQueueLengthAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get order queue length");
            OrderQueueLength = 0;
        }

        try
        {
            InventoryQueueLength = await _queueStorageService.GetInventoryQueueLengthAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get inventory queue length");
            InventoryQueueLength = 0;
        }
    }

    public async Task<IActionResult> OnPostAddCustomerAsync(string firstName, string lastName, string email, string phoneNumber, string address)
    {
        var customer = new CustomerEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address
        };

        await _tableStorageService.AddCustomerAsync(customer);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "table" });
    }

    public async Task<IActionResult> OnPostAddProductAsync(string name, string category, double price, string description, int stockQuantity)
    {
        var product = new ProductEntity
        {
            Name = name,
            Category = category,
            Price = price,
            Description = description,
            StockQuantity = stockQuantity
        };

        await _tableStorageService.AddProductAsync(product);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "table" });
    }

    public async Task<IActionResult> OnPostUploadBlobAsync(string blobName, IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            await _blobStorageService.UploadBlobAsync(blobName, stream, imageFile.ContentType);
        }

        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "blob" });
    }

    public async Task<IActionResult> OnPostDeleteBlobAsync(string blobName)
    {
        await _blobStorageService.DeleteBlobAsync(blobName);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "blob" });
    }

    public async Task<IActionResult> OnPostAddOrderAsync(string customerId, string productId, int quantity, double totalAmount)
    {
        var order = new OrderMessage
        {
            CustomerId = customerId,
            ProductId = productId,
            Quantity = quantity,
            TotalAmount = totalAmount,
            Status = "Processing"
        };

        await _queueStorageService.SendMessageToOrderQueueAsync(order);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "queue" });
    }

    public async Task<IActionResult> OnPostAddInventoryAsync(string productId, string action, int quantity, string imageName)
    {
        var inventory = new InventoryMessage
        {
            ProductId = productId,
            Action = action,
            Quantity = quantity,
            ImageName = imageName
        };

        await _queueStorageService.SendMessageToInventoryQueueAsync(inventory);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "queue" });
    }

    public async Task<IActionResult> OnPostUploadLogFileAsync(string fileName, IFormFile logFile)
    {
        if (logFile != null && logFile.Length > 0)
        {
            using var stream = logFile.OpenReadStream();
            await _fileStorageService.UploadLogFileAsync(fileName, stream);
        }

        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "file" });
    }

    public async Task<IActionResult> OnPostDeleteLogFileAsync(string fileName)
    {
        await _fileStorageService.DeleteLogFileAsync(fileName);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "file" });
    }

    public async Task<IActionResult> OnPostCreateLogEntryAsync(string logContent)
    {
        await _fileStorageService.CreateLogEntryAsync(logContent);
        await LoadDataAsync();
        return RedirectToPage("./Index", new { activeTab = "file" });
    }
}
