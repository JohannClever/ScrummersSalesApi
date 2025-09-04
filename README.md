# ScrummersSalesApi
This is a Sales microservice api
# Scrummers Sales API (Products Service)

## Overview
This project is a backend service built with **C#** and **.NET 8**.  
It is designed following **Domain-Driven Design (DDD) principles**, applying a layered architecture that separates concerns between Domain, Application, Infrastructure, and API.

The service currently runs on **Windows environments** and exposes a set of APIs for managing **Products**.

---

## Architecture
- **Domain Layer**  
  Contains the core business entities, value objects, and interfaces. The domain is technology-agnostic and represents the business rules.

- **Application Layer**  
  Coordinates the application workflow. Uses **MediatR** to implement **CQRS** (Command and Query Responsibility Segregation).  
  - **Queries** → executed with **Dapper** for fast, lightweight data retrieval.  
  - **Commands** → executed with **Entity Framework Core** (EF) for data manipulation (insert, update, delete).

- **Infrastructure Layer**  
  Handles persistence, repository implementations, EF configurations, and external services.  
  It bridges the application and domain layers with the database and external systems.

- **API Layer**  
  Exposes RESTful endpoints using **ASP.NET Core Web API**.  
  Controllers delegate requests to **MediatR handlers**, which dispatch to services in the Application layer.

---

## Key Technologies
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core** (for commands)
- **Dapper** (for queries)
- **MediatR** (for CQRS and request/response handling)
- **SQL Server** as the database
- **Serilog** for structured logging

---

## Current Status
- ✅ Runs locally on **Windows** with Visual Studio or `dotnet run`.
- ✅ Database connection via **SQL Server Express**.
- ✅ Layered architecture with **DDD focus**.  
- ✅ Commands implemented with **EF Core**.  
- ✅ Queries implemented with **Dapper**.  
- ✅ **MediatR** integration for CQRS.  
- ⏳ **Docker support is not yet available**. A Dockerfile and containerization setup are currently under development.

---

## How to Run
1. Clone the repository.
2. Ensure you have **.NET 8 SDK** installed.
3. Configure your database connection string in `appsettings.json`.
4. Run migrations or apply SQL scripts for initial schema.
5. Start the API:
   ```bash
   dotnet run --project ScrummersSalesApi.Backend.Products.Api
