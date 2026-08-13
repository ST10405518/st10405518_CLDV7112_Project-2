using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetailApp.Services;

public class TableStorageService
{
    private readonly TableClient _customerTableClient;
    private readonly TableClient _productTableClient;
    private readonly string _connectionString;

    public TableStorageService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"]!;
        
        try
        {
            var serviceClient = new TableServiceClient(_connectionString);
            
            // Create tables for customers and products
            _customerTableClient = serviceClient.GetTableClient("Customers");
            _customerTableClient.CreateIfNotExists();
            
            _productTableClient = serviceClient.GetTableClient("Products");
            _productTableClient.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            // Log the error but allow the application to start
            // Tables will be created when emulator is available
            Console.WriteLine($"Azure Storage Emulator not available: {ex.Message}");
            var serviceClient = new TableServiceClient(_connectionString);
            _customerTableClient = serviceClient.GetTableClient("Customers");
            _productTableClient = serviceClient.GetTableClient("Products");
        }
    }

    // Customer Operations
    public async Task AddCustomerAsync(CustomerEntity customer)
    {
        await _customerTableClient.AddEntityAsync(customer);
    }

    public async Task<CustomerEntity?> GetCustomerAsync(string partitionKey, string rowKey)
    {
        try
        {
            var response = await _customerTableClient.GetEntityAsync<CustomerEntity>(partitionKey, rowKey);
            return response.Value;
        }
        catch (RequestFailedException)
        {
            return null;
        }
    }

    public async Task<List<CustomerEntity>> GetAllCustomersAsync()
    {
        var customers = new List<CustomerEntity>();
        await foreach (var customer in _customerTableClient.QueryAsync<CustomerEntity>())
        {
            customers.Add(customer);
        }
        return customers;
    }

    public async Task UpdateCustomerAsync(CustomerEntity customer)
    {
        await _customerTableClient.UpdateEntityAsync(customer, customer.ETag);
    }

    public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
    {
        await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    // Product Operations
    public async Task AddProductAsync(ProductEntity product)
    {
        await _productTableClient.AddEntityAsync(product);
    }

    public async Task<ProductEntity?> GetProductAsync(string partitionKey, string rowKey)
    {
        try
        {
            var response = await _productTableClient.GetEntityAsync<ProductEntity>(partitionKey, rowKey);
            return response.Value;
        }
        catch (RequestFailedException)
        {
            return null;
        }
    }

    public async Task<List<ProductEntity>> GetAllProductsAsync()
    {
        var products = new List<ProductEntity>();
        await foreach (var product in _productTableClient.QueryAsync<ProductEntity>())
        {
            products.Add(product);
        }
        return products;
    }

    public async Task UpdateProductAsync(ProductEntity product)
    {
        await _productTableClient.UpdateEntityAsync(product, product.ETag);
    }

    public async Task DeleteProductAsync(string partitionKey, string rowKey)
    {
        await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }
}

public class CustomerEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Customer";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
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
    public string PartitionKey { get; set; } = "Product";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
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
