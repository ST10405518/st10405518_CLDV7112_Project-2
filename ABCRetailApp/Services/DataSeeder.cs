using Azure.Data.Tables;

namespace ABCRetailApp.Services;

public class DataSeeder
{
    private readonly TableStorageService _tableStorageService;
    private readonly BlobStorageService _blobStorageService;
    private readonly QueueStorageService _queueStorageService;
    private readonly FileStorageService _fileStorageService;

    public DataSeeder(
        TableStorageService tableStorageService,
        BlobStorageService blobStorageService,
        QueueStorageService queueStorageService,
        FileStorageService fileStorageService)
    {
        _tableStorageService = tableStorageService;
        _blobStorageService = blobStorageService;
        _queueStorageService = queueStorageService;
        _fileStorageService = fileStorageService;
    }

    public async Task SeedSampleDataAsync()
    {
        await SeedCustomersAsync();
        await SeedProductsAsync();
        await SeedOrdersAsync();
        await SeedInventoryMessagesAsync();
        await SeedLogFilesAsync();
    }

    private async Task SeedCustomersAsync()
    {
        try
        {
            var customers = new List<CustomerEntity>
            {
                new() { FirstName = "Thabang", LastName = "Mashile", Email = "thabang.mashile@example.com", PhoneNumber = "0821234567", Address = "123 Main Street, Johannesburg" },
                new() { FirstName = "Lindokuhle", LastName = "Zwane", Email = "lindokuhle.zwane@example.com", PhoneNumber = "0832345678", Address = "456 Oak Avenue, Cape Town" },
                new() { FirstName = "Musawenkosi", LastName = "Bhebhe", Email = "musawenkosi.bhebhe@example.com", PhoneNumber = "0843456789", Address = "789 Pine Road, Durban" },
                new() { FirstName = "Rendani", LastName = "Nekhavhambe", Email = "rendani.nekhavhambe@example.com", PhoneNumber = "0854567890", Address = "321 Elm Street, Pretoria" },
                new() { FirstName = "Nercia", LastName = "Tony", Email = "nercia.tony@example.com", PhoneNumber = "0865678901", Address = "654 Maple Drive, Port Elizabeth" },
                new() { FirstName = "Ntando", LastName = "Mhlongo", Email = "ntando.mhlongo@example.com", PhoneNumber = "0876789012", Address = "987 Cedar Lane, Bloemfontein" },
                new() { FirstName = "Thembelihle", LastName = "Njomane", Email = "thembelihle.njomane@example.com", PhoneNumber = "0887890123", Address = "159 Birch Court, East London" }
            };

            var existingCustomers = await _tableStorageService.GetAllCustomersAsync();
            if (existingCustomers.Count == 0)
            {
                foreach (var customer in customers)
                {
                    await _tableStorageService.AddCustomerAsync(customer);
                }
                Console.WriteLine($"Seeded {customers.Count} sample customers.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to seed customers: {ex.Message}");
        }
    }

    private async Task SeedProductsAsync()
    {
        try
        {
            var products = new List<ProductEntity>
            {
                new() { Name = "Premium Laptop", Category = "Electronics", Price = 15999.99, Description = "High-performance laptop with 16GB RAM, 512GB SSD", StockQuantity = 50 },
                new() { Name = "Wireless Mouse", Category = "Electronics", Price = 499.99, Description = "Ergonomic wireless mouse with precision tracking", StockQuantity = 100 },
                new() { Name = "Office Chair", Category = "Furniture", Price = 2499.99, Description = "Ergonomic office chair with adjustable height", StockQuantity = 30 },
                new() { Name = "Monitor Stand", Category = "Accessories", Price = 899.99, Description = "Aluminum monitor stand with cable management", StockQuantity = 75 },
                new() { Name = "USB-C Hub", Category = "Accessories", Price = 699.99, Description = "7-in-1 USB-C hub with HDMI and USB 3.0 ports", StockQuantity = 200 },
                new() { Name = "Mechanical Keyboard", Category = "Electronics", Price = 1299.99, Description = "RGB mechanical keyboard with Cherry MX switches", StockQuantity = 60 },
                new() { Name = "Webcam HD", Category = "Electronics", Price = 899.99, Description = "1080p HD webcam with built-in microphone", StockQuantity = 45 }
            };

            var existingProducts = await _tableStorageService.GetAllProductsAsync();
            if (existingProducts.Count == 0)
            {
                foreach (var product in products)
                {
                    await _tableStorageService.AddProductAsync(product);
                }
                Console.WriteLine($"Seeded {products.Count} sample products.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to seed products: {ex.Message}");
        }
    }

    private async Task SeedOrdersAsync()
    {
        try
        {
            var orders = new List<OrderMessage>
            {
                new() { CustomerId = "cus1001", ProductId = "prod_001", Quantity = 2, TotalAmount = 31999.98, Status = "Processing" },
                new() { CustomerId = "cus1002", ProductId = "prod_002", Quantity = 5, TotalAmount = 2499.95, Status = "Processing" },
                new() { CustomerId = "cus1003", ProductId = "prod_003", Quantity = 1, TotalAmount = 2499.99, Status = "Processing" },
                new() { CustomerId = "cus1004", ProductId = "prod_004", Quantity = 3, TotalAmount = 2699.97, Status = "Processing" },
                new() { CustomerId = "cus1005", ProductId = "prod_005", Quantity = 10, TotalAmount = 6999.90, Status = "Processing" },
                new() { CustomerId = "cus1006", ProductId = "prod_006", Quantity = 2, TotalAmount = 2599.98, Status = "Processing" },
                new() { CustomerId = "cus1007", ProductId = "prod_007", Quantity = 4, TotalAmount = 3599.96, Status = "Processing" }
            };

            var orderQueueLength = await _queueStorageService.GetOrderQueueLengthAsync();
            if (orderQueueLength == 0)
            {
                foreach (var order in orders)
                {
                    await _queueStorageService.SendMessageToOrderQueueAsync(order);
                }
                Console.WriteLine($"Seeded {orders.Count} sample orders.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to seed orders: {ex.Message}");
        }
    }

    private async Task SeedInventoryMessagesAsync()
    {
        try
        {
            var inventoryMessages = new List<InventoryMessage>
            {
                new() { ProductId = "prod_001", Action = "Update", Quantity = 10, ImageName = "laptop.jpg" },
                new() { ProductId = "prod_002", Action = "Restock", Quantity = 50, ImageName = "mouse.jpg" },
                new() { ProductId = "prod_003", Action = "Check", Quantity = 5, ImageName = "chair.jpg" },
                new() { ProductId = "prod_004", Action = "Update", Quantity = 25, ImageName = "stand.jpg" },
                new() { ProductId = "prod_005", Action = "Restock", Quantity = 100, ImageName = "hub.jpg" },
                new() { ProductId = "prod_006", Action = "Update", Quantity = 15, ImageName = "keyboard.jpg" },
                new() { ProductId = "prod_007", Action = "Check", Quantity = 20, ImageName = "webcam.jpg" }
            };

            var inventoryQueueLength = await _queueStorageService.GetInventoryQueueLengthAsync();
            if (inventoryQueueLength == 0)
            {
                foreach (var message in inventoryMessages)
                {
                    await _queueStorageService.SendMessageToInventoryQueueAsync(message);
                }
                Console.WriteLine($"Seeded {inventoryMessages.Count} sample inventory messages.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to seed inventory messages: {ex.Message}");
        }
    }

    private async Task SeedLogFilesAsync()
    {
        try
        {
            var logEntries = new List<string>
            {
                "2026-08-12 08:00:00 - Application started successfully",
                "2026-08-12 09:15:23 - Customer John Smith added new order",
                "2026-08-12 10:30:45 - Product inventory updated for prod_001",
                "2026-08-12 11:45:12 - Payment processed for order #12345",
                "2026-08-12 12:00:00 - Daily backup completed",
                "2026-08-12 13:30:18 - New customer registered: Jane Doe",
                "2026-08-12 14:45:30 - System health check passed"
            };

            var logFiles = await _fileStorageService.GetAllLogFilesAsync();
            if (logFiles.Count == 0)
            {
                foreach (var logEntry in logEntries)
                {
                    await _fileStorageService.CreateLogEntryAsync(logEntry);
                }
                Console.WriteLine($"Seeded {logEntries.Count} sample log entries.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to seed log files: {ex.Message}");
        }
    }
}
