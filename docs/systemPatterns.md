# System Patterns & Standards

## Technology Stack

### Backend
- **Framework:** .NET 8.0 (ASP.NET Core Web API)
- **Language:** C# 12
- **Database:** SQL Server (accessed via EF Core 8.0)
- **Key Libraries:**
  - `Microsoft.EntityFrameworkCore` (ORM)
  - `Serilog` (Logging)
  - `Microsoft.AspNetCore.Authentication.JwtBearer` (Auth)
  - `Swashbuckle.AspNetCore` (Swagger/OpenAPI)

### Frontend
- **Framework:** React 18.3
- **Build Tool:** Vite 5.4
- **Language:** TypeScript 5.5
- **Styling:** TailwindCSS 3.4
- **Router:** React Router Dom 6.30
- **State Management:** (To be confirmed - likely Context API or React Hooks)
- **Icons:** Lucide React

## Architecture Overview
The project follows a **Layered Architecture** (N-Tier) variant:

1.  **Presentation Layer (API):**
    - `Controllers`: Handle HTTP requests, input validation, and response mapping.
    - `Middleware`: Global error handling, rate limiting.
    - `DTOs`: Contracts for external communication (separate project).

2.  **Business Layer (Domain):**
    - `Services`: Core business logic implementation.
    - `Interfaces`: Abstractions for repositories and services.
    - `Entities`: Domain models.

3.  **Data Access Layer (DataAccess):**
    - `Repositories`: Abstraction over database operations (`GenericRepository` pattern detected).
    - `Data`: EF Core `DbContext` configurations.

## Code Conventions

### Naming
- **C# Classes/Methods/Properties:** PascalCase (e.g., `ProductService`, `GetPrice`)
- **C# Local Variables/Parameters:** camelCase (e.g., `productId`, `dbContext`)
- **C# Interfaces:** IPascalCase (e.g., `IProductRepository`)
- **TypeScript Components:** PascalCase (e.g., `ProductCard.tsx`)
- **TypeScript Functions/Vars:** camelCase

### Patterns
- **Dependency Injection:** Heavy use of Constructor Injection. Default lifetime is `Scoped` (`builder.Services.AddScoped`).
- **Authorization:** JWT Bearer tokens with Role-based checks.
- **Error Handling:** Global `ExceptionHandlingMiddleware`.
- **API Response:** Standard HTTP Status codes.

## "Red Lines" (Prohibited)
- ❌ Direct DB access from Controllers (Must use Service/Repository).
- ❌ Hardcoded usage of `DateTime.Now` (Use `DateTime.UtcNow`).
- ❌ Mixing Frontend logic (React) with Backend DTO definitions directly (Must use shared types or mapping).
