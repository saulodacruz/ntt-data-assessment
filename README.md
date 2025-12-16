# Developer Evaluation - Sales API

REST API developed in .NET 8 for sales and user management, following Domain-Driven Design (DDD) and Clean Architecture principles.

## 📋 About the Project

This API allows complete CRUD operations for sales, including:
- Sales management (create, query, update, cancel)
- Sale items management (cancel individual items)
- User management
- JWT authentication
- Automatic quantity-based discount application

### Business Rules

- **Quantity-based discounts:**
  - 4+ identical items: 10% discount
  - 10-20 identical items: 20% discount
  - Maximum of 20 identical items per product
  - No discount for quantities below 4 items

## 🛠️ Technologies

- **.NET 8.0**
- **PostgreSQL** (database)
- **Entity Framework Core** (ORM)
- **MediatR** (CQRS)
- **AutoMapper** (object mapping)
- **JWT** (authentication)
- **Swagger** (API documentation)
- **Docker & Docker Compose** (containerization)
- **Serilog** (logging)

## 📁 Project Structure

```
backend/
├── src/
│   ├── Ambev.DeveloperEvaluation.WebApi/      # Presentation layer (Controllers, Middleware)
│   ├── Ambev.DeveloperEvaluation.Application/ # Application layer (Use Cases, Handlers)
│   ├── Ambev.DeveloperEvaluation.Domain/      # Domain layer (Entities, Events, Rules)
│   ├── Ambev.DeveloperEvaluation.ORM/         # Persistence layer (DbContext, Migrations)
│   ├── Ambev.DeveloperEvaluation.Common/      # Shared utilities (Security, Validation)
│   └── Ambev.DeveloperEvaluation.IoC/         # Dependency injection
└── tests/                                      # Unit, integration and functional tests
```

## 🚀 How to Run

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (or Docker + Docker Compose)

### Option 1: Using Docker Compose (Recommended)

1. **Clone the repository and navigate to the backend folder:**
   ```bash
   cd template/backend
   ```

2. **Run Docker Compose:**
   
   The `docker-compose.yml` is already configured with the correct connection string for the Docker environment. No adjustments are needed.

3. **Start the containers:**
   ```bash
   docker-compose up -d
   ```

4. **Wait for the containers to start and run the migrations:**
   ```bash
   docker-compose exec ambev.developerevaluation.webapi dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
   ```

5. **Access the API:**
   - **Swagger UI:** http://localhost:8080/swagger
   - **API:** http://localhost:8080

### Option 2: Running Locally (without Docker)

1. **Install PostgreSQL** and create a database:
   ```sql
   CREATE DATABASE developer_evaluation;
   ```

2. **Adjust the connection string in `appsettings.json`:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=your_user;Password=your_password"
     }
   }
   ```

3. **Navigate to the WebApi project folder:**
   ```bash
   cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
   ```

4. **Run the migrations:**
   ```bash
   dotnet ef database update --project ../Ambev.DeveloperEvaluation.ORM
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

6. **Access the API:**
   - **Swagger UI:** https://localhost:5001/swagger (or http://localhost:5000/swagger)
   - **API:** https://localhost:5001

## 🔐 Authentication

The API uses JWT authentication. To access protected endpoints:

1. **Create a user** via the registration endpoint
2. **Authenticate** via the login endpoint to obtain the JWT token
3. **Use the token** in the `Authorization: Bearer {token}` header

## 📚 API Documentation

Complete API documentation is available via Swagger when the application is running:
- **URL:** `/swagger`

## 🧪 Tests

The project includes three types of tests:

- **Unit Tests:** `Ambev.DeveloperEvaluation.Unit`
- **Integration Tests:** `Ambev.DeveloperEvaluation.Integration`
- **Functional Tests:** `Ambev.DeveloperEvaluation.Functional`

To run the tests:
```bash
cd template/backend
dotnet test
```

## 📝 Domain Events

The application publishes the following domain events (currently logged):
- `SaleCreated` - When a sale is created
- `SaleModified` - When a sale is modified
- `SaleCancelled` - When a sale is cancelled
- `ItemCancelled` - When a sale item is cancelled

## 🛑 Stop Containers

To stop and remove containers:
```bash
docker-compose down
```

To stop and remove containers along with volumes (database data):
```bash
docker-compose down -v
```

## 📞 Support

If you have questions or issues, check:
- Container logs: `docker-compose logs`
- Swagger documentation
- Configuration files in `appsettings.json`
