# ABC Retail - Azure Storage Solution

**Student Number:** st10405518  
**Module Code:** CLDV7112  
**Project:** Project 1 & 2 - Azure Storage Solution with Azure Functions Integration

## Overview

This web application demonstrates the use of various Azure Storage Services for ABC Retail, including:
- **Azure Table Storage** - Customer profiles and product information
- **Azure Blob Storage** - Product images and multimedia content
- **Azure Queue Storage** - Order processing and inventory management
- **Azure Files** - Log file storage

### Project 2 Extension
Project 2 extends this application with Azure Functions that provide serverless integration with Azure Storage services:
- **Table Storage Function** - Serverless customer and product data operations
- **Blob Storage Function** - Serverless image upload and management
- **Queue Storage Function** - Serverless order and inventory message processing
- **Azure Files Function** - Serverless log file management

See the `ABCRetailFunctions` folder for the Azure Functions implementation.

## Prerequisites

### 1. Azure Storage Emulator (for local development)

To run this application locally, you need the Azure Storage Emulator:

**Option A: Using Azurite (Recommended)**
```powershell
# Install Azurite
npm install -g azurite

# Start Azurite
azurite
```

**Option B: Using Azure Storage Emulator (Windows)**
- Download and install from: https://azure.microsoft.com/downloads/
- Start the emulator from the Start Menu: "Microsoft Azure Storage Emulator"

### 2. .NET 9.0 SDK
Ensure you have .NET 9.0 SDK installed:
```powershell
dotnet --version
```

## Running the Application Locally

Once the Azure Storage Emulator is running:

```powershell
cd ABCRetailApp
dotnet run
```

The application will be available at: `http://localhost:5184`

## Adding Sample Data (5+ records each)

### Table Storage - Customers
Add at least 5 customers using the web form:
1. John Smith - john@example.com - 555-0101
2. Jane Doe - jane@example.com - 555-0102
3. Bob Johnson - bob@example.com - 555-0103
4. Alice Williams - alice@example.com - 555-0104
5. Charlie Brown - charlie@example.com - 555-0105

### Table Storage - Products
Add at least 5 products using the web form:
1. Laptop - Electronics - $999.99
2. Smartphone - Electronics - $699.99
3. Headphones - Electronics - $149.99
4. Desk Chair - Furniture - $299.99
5. Monitor - Electronics - $449.99

### Blob Storage - Images
Upload at least 5 product images:
1. laptop.jpg
2. smartphone.jpg
3. headphones.jpg
4. chair.jpg
5. monitor.jpg

### Queue Storage - Orders
Add at least 5 order messages:
1. Order for Customer 1 - Product 1 - Qty 2
2. Order for Customer 2 - Product 2 - Qty 1
3. Order for Customer 3 - Product 3 - Qty 3
4. Order for Customer 4 - Product 4 - Qty 1
5. Order for Customer 5 - Product 5 - Qty 2

### Queue Storage - Inventory
Add at least 5 inventory messages:
1. Update stock for Product 1
2. Restock Product 2 - Qty 10
3. Check stock for Product 3
4. Update stock for Product 4
5. Restock Product 5 - Qty 15

### Azure Files - Log Files
Create at least 5 log entries:
1. "System startup log - 2026-08-10"
2. "Customer login attempt - User1"
3. "Order processed successfully - Order123"
4. "Inventory update completed"
5. "System backup completed"

## Deployment to Azure App Service

This section provides detailed step-by-step instructions for deploying the ABC Retail application to Azure App Service using multiple methods.

### Prerequisites for Deployment

Before deploying, ensure you have:
- An active Azure account (free trial available at https://azure.microsoft.com/free/)
- Azure CLI installed (optional, for CLI deployment)
- Visual Studio 2022 (optional, for Visual Studio deployment)
- Git installed (for GitHub deployment)

### Method 1: Azure CLI Deployment (Recommended for Automation)

#### Step 1: Install Azure CLI
```powershell
# Install Azure CLI using winget
winget install Microsoft.AzureCLI

# Or download from: https://docs.microsoft.com/cli/azure/install-azure-cli
```

#### Step 2: Login to Azure
```powershell
az login
```
This will open a browser window for you to sign in to your Azure account.

#### Step 3: Create Resource Group
```powershell
# Create a resource group in South Africa North region
az group create --name ABCRetailRG --location southafricanorth
```

#### Step 4: Create Storage Account
```powershell
# Create a storage account (must be globally unique, replace with your name)
az storage account create `
  --name st10405518storage `
  --resource-group ABCRetailRG `
  --location southafricanorth `
  --sku Standard_LRS `
  --kind StorageV2
```

#### Step 5: Get Storage Connection String
```powershell
# Get the connection string for your storage account
az storage account show-connection-string `
  --name st10405518storage `
  --resource-group ABCRetailRG `
  --query connectionString `
  --output tsv
```
Copy this connection string - you'll need it later.

#### Step 6: Create App Service Plan
```powershell
# Create a Basic B1 App Service plan (cost-effective for testing)
az appservice plan create `
  --name ABCRetailPlan `
  --resource-group ABCRetailRG `
  --location southafricanorth `
  --sku B1 `
  --is-linux
```

#### Step 7: Create Web App
```powershell
# Create the web application
az webapp create `
  --name st10405518 `
  --resource-group ABCRetailRG `
  --plan ABCRetailPlan `
  --runtime "DOTNET|9.0"
```

#### Step 8: Configure Application Settings
```powershell
# Set the Azure Storage connection string as an app setting
az webapp config appsettings set `
  --name st10405518 `
  --resource-group ABCRetailRG `
  --settings AzureStorage__ConnectionString="YOUR_CONNECTION_STRING_HERE"
```

Replace `YOUR_CONNECTION_STRING_HERE` with the connection string from Step 5.

#### Step 9: Deploy the Application
```powershell
# Navigate to your project directory
cd C:\Users\Student\st10405518_CLDV7112_Project1\ABCRetailApp

# Publish the application
dotnet publish -c Release -o ./publish

# Deploy to Azure
az webapp deployment source config-zip `
  --name st10405518 `
  --resource-group ABCRetailRG `
  --src ./publish
```

#### Step 10: Verify Deployment
```powershell
# Browse to your application
az webapp browse --name st10405518 --resource-group ABCRetailRG
```

Your application will be available at: `https://st10405518.azurewebsites.net`

---

### Method 2: Visual Studio Deployment (Easiest for Beginners)

#### Step 1: Open Project in Visual Studio
- Open `ABCRetailApp.sln` in Visual Studio 2022

#### Step 2: Right-Click Project and Select Publish
- In Solution Explorer, right-click on the `ABCRetailApp` project
- Select "Publish"

#### Step 3: Choose Deployment Target
- Select "Azure" as the target
- Choose "Azure App Service (Windows)" or "Azure App Service (Linux)"
- Click "Next"

#### Step 4: Create New App Service (or select existing)
- Click "Create new" to create a new App Service
- Fill in the details:
  - **App name:** st10405518 (must be unique)
  - **Resource group:** Create new "ABCRetailRG"
  - **Hosting plan:** Create new "ABCRetailPlan"
  - **Region:** South Africa North
  - **Runtime:** .NET 9.0
- Click "Create"

#### Step 5: Configure Connection String
- After creating the App Service, click "Manage Application Settings"
- Add a new setting:
  - **Name:** AzureStorage__ConnectionString
  - **Value:** Your Azure Storage connection string
- Click "Save"

#### Step 6: Publish
- Click "Publish" in the Visual Studio Publish dialog
- Visual Studio will build and deploy your application
- The browser will open automatically when deployment completes

---

### Method 3: GitHub Actions Deployment (Recommended for CI/CD)

#### Step 1: Create GitHub Repository
```powershell
# Initialize git repository
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - ABC Retail Azure Storage Solution"

# Create repository on GitHub at https://github.com/new
# Then add remote and push
git remote add origin https://github.com/yourusername/st10405518_CLDV7112_Project1.git
git branch -M main
git push -u origin main
```

#### Step 2: Create Azure Resources
Follow Steps 3-7 from Method 1 (Azure CLI) to create:
- Resource Group
- Storage Account
- App Service Plan
- Web App

#### Step 3: Configure GitHub Secrets
1. Go to your GitHub repository
2. Navigate to Settings → Secrets and variables → Actions
3. Add the following secrets:
   - `AZURE_WEBAPP_NAME`: st10405518
   - `AZURE_WEBAPP_PACKAGE_PATH`: ./publish
   - `AZURE_WEBAPP_PUBLISH_PROFILE`: (Download from Azure Portal)
   - `AZURE_STORAGE_CONNECTION_STRING`: Your storage connection string

To get the publish profile:
- Go to Azure Portal → Your App Service
- Click "Get publish profile" in the Overview section
- Copy the entire XML content and add as a GitHub secret

#### Step 4: Create GitHub Actions Workflow
Create a file `.github/workflows/azure-webapp.yml`:

```yaml
name: Build and deploy ASP.NET Core app to Azure Web App

on:
  push:
    branches: [ "main" ]
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Set up .NET Core
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Build with dotnet
      run: dotnet build --configuration Release

    - name: dotnet publish
      run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp

    - name: Upload artifact for deployment job
      uses: actions/upload-artifact@v4
      with:
        name: .net-app
        path: ${{env.DOTNET_ROOT}}/myapp

  deploy:
    runs-on: ubuntu-latest
    needs: build
    environment:
      name: 'Production'
      url: ${{ steps.deploy-to-webapp.outputs.webapp-url }}

    steps:
    - name: Download artifact from build job
      uses: actions/download-artifact@v4
      with:
        name: .net-app

    - name: Deploy to Azure Web App
      id: deploy-to-webapp
      uses: azure/webapps-deploy@v3
      with:
        app-name: 'st10405518'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: .
```

#### Step 5: Push to GitHub
```powershell
git add .github/workflows/azure-webapp.yml
git commit -m "Add GitHub Actions workflow"
git push
```

The workflow will automatically build and deploy your application.

---

### Method 4: Azure Portal Manual Deployment

#### Step 1: Create Storage Account in Portal
1. Go to https://portal.azure.com
2. Click "Create a resource" → Search "Storage Account"
3. Fill in the details:
   - **Resource group:** Create new "ABCRetailRG"
   - **Storage account name:** st10405518storage (must be unique)
   - **Region:** South Africa North
   - **Performance:** Standard
   - **Redundancy:** Locally redundant storage (LRS)
4. Click "Review + create" → "Create"

#### Step 2: Get Connection String
1. Go to your storage account in the portal
2. Click "Access keys" in the left menu
3. Copy the "Connection string" under key1

#### Step 3: Create App Service in Portal
1. Click "Create a resource" → Search "Web App"
2. Fill in the details:
   - **Resource group:** ABCRetailRG
   - **App name:** st1040550418 (must be unique)
   - **Publish:** Code
   - **Runtime stack:** .NET 9.0
   - **Operating System:** Linux
   - **Region:** South Africa North
   - **App Service Plan:** Create new "ABCRetailPlan" (Basic B1)
3. Click "Review + create" → "Create"

#### Step 4: Configure App Settings
1. Go to your App Service in the portal
2. Click "Configuration" in the left menu
3. Click "New application setting"
4. Add:
   - **Name:** AzureStorage__ConnectionString
   - **Value:** Your storage connection string from Step 2
5. Click "Save"

#### Step 5: Deploy via FTP or Zip Deploy
**Option A: Zip Deploy**
```powershell
# Publish locally
dotnet publish -c Release -o ./publish

# Compress the publish folder
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip

# Use Azure CLI to deploy (requires login)
az webapp deployment source config-zip `
  --name st10405518 `
  --resource-group ABCRetailRG `
  --src ./publish.zip
```

**Option B: FTP Deployment**
1. Go to your App Service in the portal
2. Click "Deployment Center" → "FTP Credentials"
3. Copy the FTP hostname, username, and password
4. Use FileZilla or similar FTP client to upload files from `./publish` folder to `/site/wwwroot`

---

### Post-Deployment Configuration

#### Disable Data Seeding in Production
The application automatically seeds sample data in development. To disable this in production:

**Option 1: Update Program.cs**
```csharp
// Seed sample data in development only
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedSampleDataAsync();
    }
}
```

**Option 2: Use App Setting**
Add an app setting in Azure:
- **Name:** ASPNETCORE_ENVIRONMENT
- **Value:** Production

#### Verify Deployment
1. Open your browser and navigate to `https://st10405518.azurewebsites.net`
2. Test all features:
   - Add a customer
   - Add a product
   - Upload an image
   - Add order and inventory messages
   - Create log entries
3. Check Azure Portal → Storage Account → Tables/Blobs/Queues to verify data is stored

---

### Troubleshooting Deployment Issues

#### Issue: "Deployment failed"
**Solution:** Check the deployment logs in Azure Portal → App Service → Deployment Center → Log Stream

#### Issue: "Connection string error"
**Solution:** Verify the connection string is correctly set in App Settings, not in appsettings.json

#### Issue: "Runtime not supported"
**Solution:** Ensure the App Service runtime matches your .NET version (DOTNET|9.0)

#### Issue: "Application not responding"
**Solution:** Check the Application Insights logs or enable detailed error messages in Azure Portal

---

### Cost Considerations

- **Free Tier:** Azure App Service Free tier is available but has limitations
- **Basic B1:** Approximately R150/month (South Africa North region)
- **Storage Account:** Standard LRS is very cost-effective (~R50/month for small usage)
- **Total estimated cost:** ~R200/month for testing purposes

## GitHub Repository

### Create and push to GitHub:

```powershell
# Initialize git repository
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - ABC Retail Azure Storage Solution"

# Create repository on GitHub first, then:
git remote add origin https://github.com/yourusername/st10405518_CLDV7112_Project1.git
git branch -M main
git push -u origin main
```

## Screenshots Required for Submission

You need to capture screenshots of:

1. **Table Storage** - At least 5 customer records and 5 product records
2. **Blob Storage** - At least 5 uploaded images
3. **Queue Storage** - At least 5 order messages and 5 inventory messages
4. **Azure Files** - At least 5 log files
5. **Deployed Web Application** - The running application at http://st10405518.azurewebsites.net
6. **Azure Portal** - Screenshots of your Azure resources

## Project Structure

```
st10405518_CLDV7112_Project1/
├── ABCRetailApp/                  - Project 1: Web Application
│   ├── Services/
│   │   ├── TableStorageService.cs      - Azure Table Storage operations
│   │   ├── BlobStorageService.cs       - Azure Blob Storage operations
│   │   ├── QueueStorageService.cs      - Azure Queue Storage operations
│   │   └── FileStorageService.cs       - Azure Files operations
│   ├── Pages/
│   │   ├── Index.cshtml                - Main UI with all storage operations
│   │   └── Index.cshtml.cs            - Page model with handlers
│   ├── Program.cs                     - Application configuration
│   ├── appsettings.json               - Configuration settings
│   └── ABCRetailApp.csproj           - Project file
├── ABCRetailFunctions/            - Project 2: Azure Functions
│   ├── Functions/
│   │   ├── TableStorageFunction.cs     - Serverless Table Storage operations
│   │   ├── BlobStorageFunction.cs      - Serverless Blob Storage operations
│   │   ├── QueueStorageFunction.cs     - Serverless Queue Storage operations
│   │   └── FileStorageFunction.cs      - Serverless Azure Files operations
│   ├── Program.cs                     - Functions host configuration
│   ├── host.json                      - Functions host configuration
│   ├── local.settings.json             - Local development settings
│   ├── EventServicesDiscussion.md      - Discussion on Event Hubs & Service Bus
│   └── README.md                       - Functions project documentation
└── ABCRetailApp.sln               - Solution file
```

## Features Implemented

### Project 1 Features
✅ Azure Table Storage for customer profiles and products  
✅ Azure Blob Storage for product images  
✅ Azure Queue Storage for order processing and inventory  
✅ Azure Files for log file storage  
✅ Web UI with forms for all storage operations  
✅ Scalable, reliable, and cost-effective design  

### Project 2 Features
✅ Azure Functions for Table Storage operations (store/retrieve customers and products)
✅ Azure Functions for Blob Storage operations (upload/list/delete blobs)
✅ Azure Functions for Queue Storage operations (send/receive/peek messages)
✅ Azure Functions for Azure Files operations (upload/download/list log files)
✅ Serverless architecture for enhanced scalability
✅ Discussion on Azure Event Hubs and Service Bus  

## Troubleshooting

### Issue: "Storage emulator not running"
**Solution:** Start Azurite or Azure Storage Emulator before running the application

### Issue: "Connection string error"
**Solution:** Ensure your connection string is correctly configured in appsettings.json

### Issue: "Build errors"
**Solution:** Run `dotnet restore` and `dotnet build` to restore dependencies

## URL Format

The deployed application should follow this format:
`http://st10405518.azurewebsites.net`

## Submission Checklist

### Project 1 Submission
- [ ] Student number included
- [ ] Module code included
- [ ] Screenshots of Table Storage (5+ customers, 5+ products)
- [ ] Screenshots of Blob Storage (5+ images)
- [ ] Screenshots of Queue Storage (5+ orders, 5+ inventory messages)
- [ ] Screenshots of Azure Files (5+ log files)
- [ ] Screenshots of deployed web application
- [ ] URL of deployed application (http://st10405518.azurewebsites.net)
- [ ] GitHub link for source code

### Project 2 Submission (Local Testing with Azurite)
**Note:** Testing performed locally using Azurite Azure Storage Emulator due to Azure credit limitations.

- [ ] Student number included (st10405518)
- [ ] Module code included (CLDV7112)
- [ ] Code screenshots for all 4 Azure Functions
  - [ ] TableStorageFunction.cs
  - [ ] BlobStorageFunction.cs
  - [ ] QueueStorageFunction.cs
  - [ ] FileStorageFunction.cs
- [ ] Screenshots of web application with sample data
  - [ ] 5+ customers in Table Storage
  - [ ] 5+ products in Table Storage
  - [ ] 5+ images in Blob Storage
  - [ ] 5+ order messages in Queue Storage
  - [ ] 5+ inventory messages in Queue Storage
  - [ ] 5+ log files in Azure Files
- [ ] Written answers for Azure Event Hubs discussion (see EventServicesDiscussion.md)
- [ ] Written answers for Azure Service Bus discussion (see EventServicesDiscussion.md)
- [ ] Note: "Testing performed locally using Azurite Azure Storage Emulator due to Azure credit limitations"
- [ ] GitHub link for source code

## Running the Complete Solution

### Running the Web Application (Project 1)
```powershell
cd ABCRetailApp
dotnet run
```
The web application will be available at: `http://localhost:5184`

### Running the Azure Functions (Project 2)
```powershell
cd ABCRetailFunctions
func start
```
The functions will be available at: `http://localhost:7071/api`

## Azure Functions Documentation

For detailed information about the Azure Functions implementation, including:
- Function endpoints and usage
- Deployment instructions
- Testing examples
- Event services discussion

See: `ABCRetailFunctions/README.md`
