# Invexa — Smart Inventory & Billing System

Invexa is a full-stack Inventory & Billing Management System built with ASP.NET Core MVC, Entity Framework Core and SQL Server. This scaffold contains basic models, DbContext, controllers and minimal views to get started.

Getting started

1. Ensure .NET 8 SDK is installed.
2. From the project folder run:

```powershell
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Default connection string uses LocalDB. Update `appsettings.json` to point to your SQL Server if needed.
