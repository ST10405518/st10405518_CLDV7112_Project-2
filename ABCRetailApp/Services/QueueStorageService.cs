using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;

namespace ABCRetailApp.Services;

public class QueueStorageService
{
    private readonly QueueClient _orderQueueClient;
    private readonly QueueClient _inventoryQueueClient;
    private readonly string _connectionString;

    public QueueStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"]!;
        
        try
        {
            var queueServiceClient = new QueueServiceClient(_connectionString);
            
            // Create queues for orders and inventory
            _orderQueueClient = queueServiceClient.GetQueueClient("orders");
            _orderQueueClient.CreateIfNotExists();
            
            _inventoryQueueClient = queueServiceClient.GetQueueClient("inventory");
            _inventoryQueueClient.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            // Log the error but allow the application to start
            Console.WriteLine($"Azure Storage Emulator not available: {ex.Message}");
            var queueServiceClient = new QueueServiceClient(_connectionString);
            _orderQueueClient = queueServiceClient.GetQueueClient("orders");
            _inventoryQueueClient = queueServiceClient.GetQueueClient("inventory");
        }
    }

    // Order Queue Operations
    public async Task SendMessageToOrderQueueAsync(OrderMessage message)
    {
        var messageContent = JsonSerializer.Serialize(message);
        await _orderQueueClient.SendMessageAsync(messageContent);
    }

    public async Task<OrderMessage?> ReceiveMessageFromOrderQueueAsync()
    {
        var response = await _orderQueueClient.ReceiveMessageAsync();
        if (response.Value == null)
        {
            return null;
        }

        var messageContent = response.Value.MessageText;
        await _orderQueueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
        
        return JsonSerializer.Deserialize<OrderMessage>(messageContent);
    }

    public async Task<List<PeekedMessage>> PeekOrderQueueMessagesAsync(int maxMessages = 32)
    {
        var messages = new List<PeekedMessage>();
        var response = await _orderQueueClient.PeekMessagesAsync(maxMessages);
        
        foreach (var message in response.Value)
        {
            messages.Add(message);
        }
        
        return messages;
    }

    public async Task<int> GetOrderQueueLengthAsync()
    {
        var properties = await _orderQueueClient.GetPropertiesAsync();
        return properties.Value.ApproximateMessagesCount;
    }

    // Inventory Queue Operations
    public async Task SendMessageToInventoryQueueAsync(InventoryMessage message)
    {
        var messageContent = JsonSerializer.Serialize(message);
        await _inventoryQueueClient.SendMessageAsync(messageContent);
    }

    public async Task<InventoryMessage?> ReceiveMessageFromInventoryQueueAsync()
    {
        var response = await _inventoryQueueClient.ReceiveMessageAsync();
        if (response.Value == null)
        {
            return null;
        }

        var messageContent = response.Value.MessageText;
        await _inventoryQueueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
        
        return JsonSerializer.Deserialize<InventoryMessage>(messageContent);
    }

    public async Task<List<PeekedMessage>> PeekInventoryQueueMessagesAsync(int maxMessages = 32)
    {
        var messages = new List<PeekedMessage>();
        var response = await _inventoryQueueClient.PeekMessagesAsync(maxMessages);
        
        foreach (var message in response.Value)
        {
            messages.Add(message);
        }
        
        return messages;
    }

    public async Task<int> GetInventoryQueueLengthAsync()
    {
        var properties = await _inventoryQueueClient.GetPropertiesAsync();
        return properties.Value.ApproximateMessagesCount;
    }
}

public class OrderMessage
{
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; } = "Processing";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class InventoryMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "Update", "Restock", "Check"
    public int Quantity { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
