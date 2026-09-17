using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

public class TableStorageFunction
{
    private readonly ILogger<TableStorageFunction> _logger;
    private readonly string _connectionString;

    public TableStorageFunction(ILogger<TableStorageFunction> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("AzureStorage__ConnectionString") ?? "UseDevelopmentStorage=true";
    }

    public async Task<HttpResponseData> StoreCustomer(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "table/customer")] HttpRequestData req)
    {
        _logger.LogInformation("Processing customer storage request");

        try
        {
            var customer = await req.ReadFromJsonAsync<CustomerEntity>();
            
            if (customer == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid customer data");
                return badResponse;
            }

            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient("Customers");
            await tableClient.CreateIfNotExistsAsync();

            customer.PartitionKey = "Customer";
            customer.RowKey = Guid.NewGuid().ToString();

            await tableClient.AddEntityAsync(customer);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Customer stored successfully with RowKey: {customer.RowKey}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error storing customer: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("StoreProduct")]
    public async Task<HttpResponseData> StoreProduct(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "table/product")] HttpRequestData req)
    {
        _logger.LogInformation("Processing product storage request");

        try
        {
            var product = await req.ReadFromJsonAsync<ProductEntity>();
            
            if (product == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid product data");
                return badResponse;
            }

            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient("Products");
            await tableClient.CreateIfNotExistsAsync();

            product.PartitionKey = "Product";
            product.RowKey = Guid.NewGuid().ToString();

            await tableClient.AddEntityAsync(product);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Product stored successfully with RowKey: {product.RowKey}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error storing product: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetCustomers")]
    public async Task<HttpResponseData> GetCustomers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "table/customers")] HttpRequestData req)
    {
        _logger.LogInformation("Retrieving all customers");

        try
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient("Customers");

            var customers = new List<CustomerEntity>();
            await foreach (var customer in tableClient.QueryAsync<CustomerEntity>())
            {
                customers.Add(customer);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving customers: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetProducts")]
    public async Task<HttpResponseData> GetProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "table/products")] HttpRequestData req)
    {
        _logger.LogInformation("Retrieving all products");

        try
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient("Products");

            var products = new List<ProductEntity>();
            await foreach (var product in tableClient.QueryAsync<ProductEntity>())
            {
                products.Add(product);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving products: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }
}

public class CustomerEntity : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class ProductEntity : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    public double Price { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}
