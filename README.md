# ScrummersSalesApi

This is a Sales microservices API.

---

# Scrummers Sales API (Products and Orders Services)

## Overview
This solution contains two backend microservices built with **C#** and **.NET 8**:  
- **Products Service** → manages the product catalog.  
- **Orders Service** → manages sales orders and validates against Products.  

The project follows **Domain-Driven Design (DDD) principles** with a layered architecture that separates **Domain**, **Application**, **Infrastructure**, and **API**.

---

## Architecture
- **Domain Layer**  
  Contains core business entities, value objects, and interfaces (business rules).  

- **Application Layer**  
  Coordinates workflows using **MediatR** to implement **CQRS**.  
  - Queries → with **Dapper** for fast reads.  
  - Commands → with **Entity Framework Core** for persistence.  

- **Infrastructure Layer**  
  Handles database persistence, repository implementations, EF configurations, and communication with external systems (e.g., Product service from Orders).  

- **API Layer**  
  Exposes RESTful endpoints via **ASP.NET Core Web API**. Controllers delegate requests to MediatR handlers.  

---

## Key Technologies
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core** (commands)
- **Dapper** (queries)
- **MediatR** (CQRS)
- **SQL Server** (database)
- **Polly + IHttpClientFactory** (resilient inter-service communication)
- **Serilog** (structured logging)

---

## Current Status
- ✅ Runs locally on **Windows** with Visual Studio or `dotnet run`.  
- ✅ **Products Service** and **Orders Service** run independently.  
- ✅ Products ↔ Orders integration via **HTTP + Polly**.  
- ⏳ **Docker support is not yet complete**. A Dockerfile and containerization setup are under development.  

---

## How to Run Locally (Visual Studio)

1. **Clone the repository.**

2. **Configure your database connection strings.**  
   - Open `appsettings.json` in each microservice (`Products.Api`, `Orders.Api`).  
   - Update the `"ConnectionStrings:database"` key to point to your SQL Server instance. Example:
     ```json
     "ConnectionStrings": {
       "database": "Server=localhost\\SQLEXPRESS;Database=Products;Trusted_Connection=True;Encrypt=False;"
     }
     ```

3. **Run the microservices in order:**
   - Start **Products Service** first.  
   - Then start **Orders Service** (it depends on Products for stock/price validation).  

4. **Test the endpoints (recommended order):**
   - Use Swagger (auto-enabled) at `/swagger`.  
   - First test **GET all** methods:  
     - `/Products`  
     - `/api/orders`  
   - Once you confirm data is available, test POST and PUT methods.  

5. **Running from Visual Studio:**  
   - Right-click the API project (`ScrummersSalesApi.Backend.Products.Api` or `ScrummersSalesApi.Backend.Orders.Api`).  
   - Select **Set as Startup Project**.  
   - Run with **F5** or **Ctrl+F5**.  

6. **Seed Data:**  
   Both services include **seeders** to create initial tables and insert sample data on first run.  

---

## Documentation

- **[Payment Gateway Design](docs/PaymentGatewayDesign.md)** - Comprehensive design document for implementing a payment processing system with multiple gateway support (Stripe, PayPal, Cash). Includes architecture diagrams, code examples, security considerations, and integration guidelines.

---

## Next Steps
- Add full Docker support with Compose to orchestrate Products + Orders + SQL Server.  
- Add integration tests across services.  
- Implement authentication/authorization between services.  
- Implement payment gateway integration (see [Payment Gateway Design](docs/PaymentGatewayDesign.md)).

