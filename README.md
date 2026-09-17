# ABC Retail Azure Functions - Project 2

**Student Number:** st10405518  
**Module Code:** CLDV7112  
**Project:** Project 2 - Integrating Azure Services into a Web Application

## Overview

This Azure Functions project extends the ABC Retail web application by implementing serverless functions that interact with Azure Storage services. These functions demonstrate cloud-native architecture patterns and enhance scalability, cost-effectiveness, and cloud suitability.

## Azure Functions Implemented

### 1. Table Storage Function
**File:** `Functions/TableStorageFunction.cs`

**Functions:**
- `StoreCustomer` - POST `/api/table/customer` - Stores customer information in Azure Table Storage
- `StoreProduct` - POST `/api/table/product` - Stores product information in Azure Table Storage
- `GetCustomers` - GET `/api/table/customers` - Retrieves all customers from Table Storage
- `GetProducts` - GET `/api/table/products` - Retrieves all products from Table Storage

**Screenshots Required:**
- Azure Function App showing the Table Storage function
- Code screenshot of the function implementation
- Azure Portal showing data in the Customers table (5+ records)
- Azure Portal showing data in the Products table (5+ records)

### 2. Blob Storage Function
**File:** `Functions/BlobStorageFunction.cs`

**Functions:**
- `UploadBlob` - POST `/api/blob/upload` - Uploads files to Azure Blob Storage
- `GetBlobs` - GET `/api/blob/list` - Lists all blobs in the container
- `DeleteBlob` - DELETE `/api/blob/{blobName}` - Deletes a specific blob
- `GetBlobUrl` - GET `/api/blob/{blobName}/url` - Gets the URL of a specific blob

**Screenshots Required:**
- Azure Function App showing the Blob Storage function
- Code screenshot of the function implementation
- Azure Portal showing 5+ uploaded images in Blob Storage

### 3. Queue Storage Function
**File:** `Functions/QueueStorageFunction.cs`

**Functions:**
- `SendOrderMessage` - POST `/api/queue/order` - Sends order messages to the order queue
- `SendInventoryMessage` - POST `/api/queue/inventory` - Sends inventory messages to the inventory queue
- `ReceiveOrderMessage` - GET `/api/queue/order/receive` - Receives and processes an order message
- `PeekOrderQueue` - GET `/api/queue/order/peek` - Peeks at messages in the order queue
- `PeekInventoryQueue` - GET `/api/queue/inventory/peek` - Peeks at messages in the inventory queue
- `GetQueueLength` - GET `/api/queue/{queueName}/length` - Gets the message count for a queue

**Screenshots Required:**
- Azure Function App showing the Queue Storage function
- Code screenshot of the function implementation
- Azure Portal showing 5+ messages in the orders queue
- Azure Portal showing 5+ messages in the inventory queue

### 4. Azure Files Function
**File:** `Functions/FileStorageFunction.cs`

**Functions:**
- `UploadLogFile` - POST `/api/file/upload` - Uploads log files to Azure Files
- `CreateLogEntry` - POST `/api/file/log` - Creates a new log entry with timestamp
- `GetLogFiles` - GET `/api/file/list` - Lists all log files in Azure Files
- `DownloadLogFile` - GET `/api/file/{fileName}` - Downloads a specific log file
- `DeleteLogFile` - DELETE `/api/file/{fileName}` - Deletes a specific log file

**Screenshots Required:**
- Azure Function App showing the Azure Files function
- Code screenshot of the function implementation
- Azure Portal showing 5+ log files in Azure Files

## Prerequisites

### 1. Azure Storage Account
You need an Azure Storage Account with the following services enabled:
- Table Storage
- Blob Storage
- Queue Storage
- Azure Files

### 2. Azure Functions Tools
Install Azure Functions Core Tools:
```powershell
npm install -g azure-functions-core-tools@4 --unsafe-perm
```

### 3. .NET 9.0 SDK
Ensure you have .NET 9.0 SDK installed:
```powershell
dotnet --version
```

## Running the Functions Locally

### Step 1: Start Azure Storage Emulator
```powershell
# Start Azurite
azurite
```

### Step 2: Configure Connection String
Update `local.settings.json` with your connection string:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureStorage__ConnectionString": "UseDevelopmentStorage=true"
  }
}
```

### Step 3: Run the Functions
```powershell
cd ABCRetailFunctions
func start
```

The functions will be available at:
- `http://localhost:7071/api/table/customer`
- `http://localhost:7071/api/blob/upload`
- `http://localhost:7071/api/queue/order`
- `http://localhost:7071/api/file/upload`

## Testing the Functions

### Test Table Storage Function
```powershell
# Store a customer
Invoke-RestMethod -Method Post -Uri "http://localhost:7071/api/table/customer" `
  -ContentType "application/json" `
  -Body '{"firstName":"John","lastName":"Smith","email":"john@example.com","phoneNumber":"555-0101","address":"123 Main St"}'

# Get all customers
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/table/customers"
```

### Test Blob Storage Function
```powershell
# Upload a blob
Invoke-RestMethod -Method Post -Uri "http://localhost:7071/api/blob/upload" `
  -ContentType "application/json" `
  -Body '{"blobName":"laptop.jpg","content":"base64encodedimage","contentType":"image/jpeg"}'

# List all blobs
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/blob/list"
```

### Test Queue Storage Function
```powershell
# Send an order message
Invoke-RestMethod -Method Post -Uri "http://localhost:7071/api/queue/order" `
  -ContentType "application/json" `
  -Body '{"customerId":"cust1","productId":"prod1","quantity":2,"totalAmount":1999.98}'

# Peek at order queue
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/queue/order/peek"
```

### Test Azure Files Function
```powershell
# Create a log entry
Invoke-RestMethod -Method Post -Uri "http://localhost:7071/api/file/log" `
  -ContentType "application/json" `
  -Body '{"content":"System startup log - 2026-09-16"}'

# List all log files
Invoke-RestMethod -Method Get -Uri "http://localhost:7071/api/file/list"
```

## Deploying to Azure

### Step 1: Create Azure Function App
```powershell
az functionapp create `
  --name st10405518functions `
  --resource-group ABCRetailRG `
  --consumption-plan-location southafricanorth `
  --runtime dotnet `
  --runtime-version 9.0 `
  --functions-version 4 `
  --storage-account st10405518storage
```

### Step 2: Configure Application Settings
```powershell
az functionapp config appsettings set `
  --name st10405518functions `
  --resource-group ABCRetailRG `
  --settings AzureStorage__ConnectionString="YOUR_CONNECTION_STRING"
```

### Step 3: Deploy the Functions
```powershell
func azure functionapp publish st10405518functions
```

## Event Services Discussion

A detailed discussion on Azure Event Hubs and Azure Service Bus (Event Bus) is provided in `EventServicesDiscussion.md`. This document includes:

### Azure Event Hubs
- **Description:** Big data streaming platform and event ingestion service
- **Mechanism:** Publish-subscribe model with partitions and consumer groups
- **Value to End Users:** Real-time order processing, personalized recommendations, inventory visibility, faster checkout, omnichannel integration, fraud detection

### Azure Service Bus (Event Bus)
- **Description:** Enterprise message broker with queues and topics
- **Mechanism:** Point-to-point queues and publish-subscribe topics with advanced features
- **Value to End Users:** Order confirmation reliability, asynchronous order processing, multi-channel notifications, order tracking, inventory restocking alerts, customer service integration, promotional campaigns, error handling

## Project Structure

```
ABCRetailFunctions/
├── Functions/
│   ├── TableStorageFunction.cs      - Azure Table Storage operations
│   ├── BlobStorageFunction.cs       - Azure Blob Storage operations
│   ├── QueueStorageFunction.cs      - Azure Queue Storage operations
│   └── FileStorageFunction.cs       - Azure Files operations
├── Program.cs                       - Functions host configuration
├── host.json                        - Functions host configuration
├── local.settings.json              - Local development settings
├── EventServicesDiscussion.md       - Discussion on Event Hubs and Service Bus
├── ABCRetailFunctions.csproj        - Project file
└── README.md                        - This file
```

## Screenshots Required for Project 2 Submission

### Function App Screenshots
1. **Table Storage Function**
   - Screenshot of the function in Azure Function App
   - Code screenshot of TableStorageFunction.cs
   - Azure Portal showing Customers table with 5+ records
   - Azure Portal showing Products table with 5+ records

2. **Blob Storage Function**
   - Screenshot of the function in Azure Function App
   - Code screenshot of BlobStorageFunction.cs
   - Azure Portal showing product-images container with 5+ images

3. **Queue Storage Function**
   - Screenshot of the function in Azure Function App
   - Code screenshot of QueueStorageFunction.cs
   - Azure Portal showing orders queue with 5+ messages
   - Azure Portal showing inventory queue with 5+ messages

4. **Azure Files Function**
   - Screenshot of the function in Azure Function App
   - Code screenshot of FileStorageFunction.cs
   - Azure Portal showing log-files share with 5+ log files

### Web Application Screenshots
5. **Deployed Web Application**
   - Screenshot of the running web application at http://st10405518.azurewebsites.net
   - Screenshots of all features working (customers, products, images, queues, files)

### Discussion Questions
6. **Event Services Discussion**
   - Written answers for Azure Event Hubs discussion (see EventServicesDiscussion.md)
   - Written answers for Azure Service Bus discussion (see EventServicesDiscussion.md)

## Submission Document Format

Your submission document must be named: `st10405518_CLDV7112_Project2.docx`

The document must include:
- Student number: st10405518
- Module code: CLDV7112
- URL of deployed application: http://st10405518.azurewebsites.net
- GitHub link for source code
- All screenshots listed above
- Written answers for discussion questions

## Testing Checklist

Before submission, ensure:
- [ ] All 4 Azure Functions are deployed and working
- [ ] Table Storage function can store and retrieve customers and products
- [ ] Blob Storage function can upload and list images
- [ ] Queue Storage function can send and receive messages
- [ ] Azure Files function can create and list log files
- [ ] Web application is deployed and accessible
- [ ] All screenshots are captured
- [ ] Discussion questions are answered
- [ ] Submission document is properly formatted

## Troubleshooting

### Issue: "Functions runtime not starting"
**Solution:** Ensure Azure Functions Core Tools is installed and the correct runtime version is specified in local.settings.json

### Issue: "Connection string error"
**Solution:** Verify the AzureStorage__ConnectionString is correctly set in local.settings.json or Azure App Settings

### Issue: "Build errors"
**Solution:** Run `dotnet restore` and `dotnet build` to restore dependencies

### Issue: "Storage emulator not running"
**Solution:** Start Azurite or Azure Storage Emulator before running functions locally

## Cost Considerations

- **Azure Functions Consumption Plan:** Pay-per-execution model, very cost-effective for testing
- **Storage Account:** Standard LRS is cost-effective (~R50/month for small usage)
- **Estimated total cost:** ~R100/month for testing purposes

## Integration with Web Application

These Azure Functions can be called from the ABC Retail web application to:
- Offload storage operations to serverless functions
- Improve scalability during peak traffic
- Reduce costs with pay-per-execution pricing
- Enable independent scaling of different components

The web application can call these functions using HTTP requests, providing a clean separation of concerns and enabling each component to scale independently.
