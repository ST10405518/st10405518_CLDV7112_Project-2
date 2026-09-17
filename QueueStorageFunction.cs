using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;

public class QueueStorageFunction
{
    private readonly ILogger<QueueStorageFunction> _logger;
    private readonly string _connectionString;

    public QueueStorageFunction(ILogger<QueueStorageFunction> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureStorage__ConnectionString") ?? "UseDevelopmentStorage=true";
    }

    [Function("SendOrderMessage")]
    public async Task<HttpResponseData> SendOrderMessage(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "queue/order")] HttpRequestData req)
    {
        _logger.LogInformation("Processing order message");

        try
        {
            var orderMessage = await req.ReadFromJsonAsync<OrderMessage>();
            
            if (orderMessage == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid order message data");
                return badResponse;
            }

            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient("orders");
            await queueClient.CreateIfNotExistsAsync();

            var messageContent = JsonSerializer.Serialize(orderMessage);
            await queueClient.SendMessageAsync(messageContent);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Order message sent successfully: {orderMessage.OrderId}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending order message: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("SendInventoryMessage")]
    public async Task<HttpResponseData> SendInventoryMessage(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "queue/inventory")] HttpRequestData req)
    {
        _logger.LogInformation("Processing inventory message");

        try
        {
            var inventoryMessage = await req.ReadFromJsonAsync<InventoryMessage>();
            
            if (inventoryMessage == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid inventory message data");
                return badResponse;
            }

            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient("inventory");
            await queueClient.CreateIfNotExistsAsync();

            var messageContent = JsonSerializer.Serialize(inventoryMessage);
            await queueClient.SendMessageAsync(messageContent);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Inventory message sent successfully: {inventoryMessage.MessageId}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending inventory message: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("ReceiveOrderMessage")]
    public async Task<HttpResponseData> ReceiveOrderMessage(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "queue/order/receive")] HttpRequestData req)
    {
        _logger.LogInformation("Receiving order message");

        try
        {
            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient("orders");

            var response = await queueClient.ReceiveMessageAsync();
            if (response.Value == null)
            {
                var noMessageResponse = req.CreateResponse(HttpStatusCode.NoContent);
                await noMessageResponse.WriteStringAsync("No messages in queue");
                return noMessageResponse;
            }

            var messageContent = response.Value.MessageText;
            await queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
            
            var orderMessage = JsonSerializer.Deserialize<OrderMessage>(messageContent);

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(orderMessage);
            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error receiving order message: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("PeekOrderQueue")]
    public async Task<HttpResponseData> PeekOrderQueue(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "queue/order/peek")] HttpRequestData req)
    {
        _logger.LogInformation("Peeking order queue");

        try
        {
            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient("orders");

            var messages = new List<PeekedMessage>();
            var response = await queueClient.PeekMessagesAsync(32);
            
            foreach (var message in response.Value)
            {
                messages.Add(message);
            }

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(messages);
            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error peeking order queue: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("PeekInventoryQueue")]
    public async Task<HttpResponseData> PeekInventoryQueue(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "queue/inventory/peek")] HttpRequestData req)
    {
        _logger.LogInformation("Peeking inventory queue");

        try
        {
            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient("inventory");

            var messages = new List<PeekedMessage>();
            var response = await queueClient.PeekMessagesAsync(32);
            
            foreach (var message in response.Value)
            {
                messages.Add(message);
            }

            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(messages);
            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error peeking inventory queue: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetQueueLength")]
    public async Task<HttpResponseData> GetQueueLength(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "queue/{queueName}/length")] HttpRequestData req,
        string queueName)
    {
        _logger.LogInformation($"Getting queue length for: {queueName}");

        try
        {
            var queueServiceClient = new QueueServiceClient(_connectionString);
            var queueClient = queueServiceClient.GetQueueClient(queueName);

            var properties = await queueClient.GetPropertiesAsync();
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { 
                queueName = queueName,
                messageCount = properties.Value.ApproximateMessagesCount 
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting queue length: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
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
    public string Action { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
