# ABC Retail - Azure Storage Solution

![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-blue.svg)
![Azure](https://img.shields.io/badge/Azure-Storage-blue.svg)

A modern web application demonstrating the use of Azure Storage Services for ABC Retail's order processing system. This project addresses scalability, reliability, and cost-effectiveness challenges faced by traditional on-premises infrastructure.

## 🚀 Live Demo

**Deployed Application:** http://st10405518.azurewebsites.net

## 📋 Project Overview

ABC Retail faced challenges with their aging on-premises infrastructure:
- Traditional relational databases struggling with peak transaction volumes
- Network shared drives causing storage inefficiencies and slow access times
- Legacy middleware lacking scalability and reliability
- Delays in order processing and inconsistent messaging delivery

This solution leverages Azure Storage Services to provide:
- **Scalable** storage that handles peak shopping seasons
- **Reliable** message queuing with guaranteed delivery
- **Cost-effective** cloud-native architecture
- **Real-time** event processing capabilities

## 🏗️ Architecture

### Azure Storage Services Used

| Service | Purpose | Implementation |
|---------|---------|----------------|
| **Azure Table Storage** | Customer profiles and product information | CRUD operations for customers and products |
| **Azure Blob Storage** | Product images and multimedia content | Upload, download, and display images |
| **Azure Queue Storage** | Order processing and inventory management | Asynchronous message processing |
| **Azure Files** | Log file storage | Centralized log management |

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 9.0 (Razor Pages)
- **Language:** C# 12
- **Storage SDKs:** Azure.Data.Tables, Azure.Storage.Blobs, Azure.Storage.Queues, Azure.Storage.Files
- **Frontend:** Bootstrap 5 with custom modern styling
- **Development:** .NET 9.0 SDK, Azurite (local emulator)

## 📦 Features

### ✅ Implemented Features

- **Customer Management**
  - Add customer profiles with name, email, phone, and address
  - View all customers in a responsive table
  - Delete customer records

- **Product Management**
  - Add products with name, category, price (in Rands), description, and stock quantity
  - View all products with pricing in South African Rands (R)
  - Delete product records

- **Image Management (Blob Storage)**
  - Upload product images
  - Display uploaded images
  - Delete images from storage

- **Order Processing (Queue Storage)**
  - Add orders with customer ID, product ID, quantity, and total amount
  - View order queue messages in formatted, readable format
  - Track order status (Processing, Completed, etc.)

- **Inventory Management (Queue Storage)**
  - Add inventory messages (Update, Restock, Check actions)
  - View inventory queue messages with product details
  - Track inventory changes in real-time

- **Log Management (Azure Files)**
  - Upload log files
  - Create log entries directly
  - View all log files with metadata
  - Delete log files

### 🎨 UI Features

- Modern, clean design without emojis
- Tab-based navigation for different storage services
- Responsive layout for mobile and desktop
- Form validation and error handling
- Tab persistence after form submission
- Real-time data display

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Azurite (Azure Storage Emulator) for local development
- Git

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/st10405518_CLDV7112_Project1.git
cd st10405518_CLDV7112_Project1/ABCRetailApp
```

2. Install Azurite (if not already installed):
```bash
npm install -g azurite
```

3. Start Azurite:
```bash
azurite --blobHost 127.0.0.1 --blobPort 10000 --queueHost 127.0.0.1 --queuePort 10001 --tableHost 127.0.0.1 --tablePort 10002 --location .azurite --debug .azurite/debug.log --skipApiVersionCheck
```

4. Restore dependencies:
```bash
dotnet restore
```

5. Run the application:
```bash
dotnet run
```

6. Open your browser and navigate to:
```
http://localhost:5184
```

## 📊 Sample Data

The application automatically seeds sample data on startup in development mode:

- **7 Customers** with South African names
- **7 Products** with pricing in Rands
- **7 Order messages** in the order queue
- **7 Inventory messages** in the inventory queue
- **7 Log entries** in the file storage

## 🔧 Configuration

### Local Development

Update `appsettings.json` with your Azurite connection string:
```json
{
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFp2F6CvqR9ytbUCF7l3TgXW2Kzd1TEtXJy2FsoTqnUwLvK3Jw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"
  }
}
```

### Production Deployment

Update the connection string in Azure App Service settings:
```
AzureStorage__ConnectionString = "Your Azure Storage connection string"
```

## 🌐 Deployment

### Azure App Service Deployment

See [README.md](README.md) for detailed deployment instructions using:
- Azure CLI
- Visual Studio
- GitHub Actions
- Azure Portal

### GitHub Actions

The repository includes a GitHub Actions workflow for automated CI/CD:
- Builds the application on push to main branch
- Runs tests
- Deploys to Azure App Service

## 📁 Project Structure

```
ABCRetailApp/
├── Services/
│   ├── TableStorageService.cs      # Azure Table Storage operations
│   ├── BlobStorageService.cs       # Azure Blob Storage operations
│   ├── QueueStorageService.cs      # Azure Queue Storage operations
│   ├── FileStorageService.cs       # Azure Files operations
│   └── DataSeeder.cs              # Sample data seeding
├── Pages/
│   ├── Index.cshtml                # Main UI with all storage operations
│   └── Index.cshtml.cs            # Page model with handlers
├── Program.cs                     # Application configuration
├── appsettings.json               # Configuration settings
├── ABCRetailApp.csproj           # Project file
├── README.md                      # Assignment documentation
└── GITHUB_README.md              # This file
```

## 🧪 Testing

### Local Testing

1. Start Azurite
2. Run the application
3. Test all features:
   - Add 5+ customers
   - Add 5+ products
   - Upload 5+ images
   - Add 5+ order messages
   - Add 5+ inventory messages
   - Create 5+ log entries

### Azure Testing

1. Deploy to Azure App Service
2. Update connection string in Azure settings
3. Test all features in the deployed environment
4. Verify data is stored in Azure Storage

## 📝 Assignment Requirements

### ✅ Completed Requirements

- [x] Student number: st10405518
- [x] Module code: CLDV7112
- [x] Azure Table Storage for customer profiles (5+ records)
- [x] Azure Table Storage for product information (5+ records)
- [x] Azure Blob Storage for images (5+ records)
- [x] Azure Queue Storage for order processing (5+ records)
- [x] Azure Queue Storage for inventory management (5+ records)
- [x] Azure Files for log storage (5+ records)
- [x] Web application with upload/download/display controls
- [x] Scalable, reliable, and cost-effective design
- [x] Local testing completed
- [x] Azure App Service deployment documentation
- [x] URL format: http://st10405518.azurewebsites.net
- [x] GitHub repository setup documentation

### 📋 Submission Checklist

- [ ] Screenshots of Table Storage (5+ customers, 5+ products)
- [ ] Screenshots of Blob Storage (5+ images)
- [ ] Screenshots of Queue Storage (5+ orders, 5+ inventory messages)
- [ ] Screenshots of Azure Files (5+ log files)
- [ ] Screenshot of deployed web application
- [ ] URL of deployed application: http://st10405518.azurewebsites.net
- [ ] GitHub link: https://github.com/yourusername/st10405518_CLDV7112_Project1

## 🐛 Troubleshooting

### Common Issues

**Issue: "Storage emulator not running"**
- Solution: Start Azurite before running the application

**Issue: "API version not supported by Azurite"**
- Solution: Start Azurite with `--skipApiVersionCheck` flag

**Issue: "Connection string error"**
- Solution: Verify connection string in appsettings.json or Azure App Settings

**Issue: "Price displays as $0.00"**
- Solution: Ensure price field is filled correctly (fixed in current version)

## 📄 License

This project is submitted as part of the CLDV7112 module at The Independent Institute of Education (Pty) Ltd.

## 👤 Author

**Student Number:** st10405518  
**Module:** CLDV7112  
**Institution:** The Independent Institute of Education (Pty) Ltd  
**Year:** 2026

## 🙏 Acknowledgments

- Microsoft Azure Documentation
- Azure Storage SDKs
- ASP.NET Core Documentation
- Bootstrap Framework
