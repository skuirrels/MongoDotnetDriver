# MongoDB .NET Driver Demo API

This is a demo Web API application built with ASP.NET Core that demonstrates CRUD operations using MongoDB and the official MongoDB .NET Driver 3.4.0 with clean domain models and GUID identifiers.

## Features

- **Order Management**: Complete CRUD operations for orders
- **Order Lines**: Support for multiple order lines per order
- **MongoDB Integration**: Uses official MongoDB .NET Driver 3.4.0 for data persistence
- **RESTful API**: Standard REST endpoints for all operations
- **Clean Domain Models**: Domain entities free from infrastructure concerns
- **GUID Identifiers**: Type-safe GUID identifiers with MongoDB BSON conversion
- **Data Validation**: Model validation with FluentValidation
- **Error Handling**: Comprehensive error handling and logging
- **Data Seeding**: Automatic database seeding with sample data on first startup
- **Unit Testing**: Comprehensive test suite validating domain logic and repository implementation

## Prerequisites

- .NET 9.0 SDK
- MongoDB server running on localhost:27017 (or update connection string)

## Project Structure

```
MongoDBDriver/
├── MongoDBDriver.API/                  # Web API Layer
│   ├── Endpoints/
│   │   └── OrderEndpoints.cs    # Minimal API endpoints for Orders
│   ├── Program.cs               # Application startup with MongoDB configuration
│   └── appsettings.json         # Configuration with MongoDB connection string
├── MongoDBDriver.Application/          # Application Layer (CQRS)
│   ├── Commands/                # Command handlers
│   ├── Queries/                 # Query handlers
│   ├── DTOs/                    # Data Transfer Objects
│   └── Validators/              # FluentValidation rules
├── MongoDBDriver.Domain/               # Domain Layer (DDD)
│   ├── Entities/                # Domain entities (Order, OrderLine)
│   ├── ValueObjects/            # Value objects (Money, CustomerName, etc.)
│   ├── Repositories/            # Repository interfaces
│   └── DomainServices/          # Domain services
├── MongoDBDriver.Infrastructure/       # Infrastructure Layer
│   ├── Documents/               # MongoDB document models with BSON attributes
│   ├── Services/                # MongoDB service implementations
│   ├── Repositories/            # Repository implementations using MongoDB.Driver
│   └── Configuration/           # MongoDB configuration settings
└── MongoDBDriver.Tests/                # Unit Tests
    ├── DomainModelTests.cs      # Domain logic validation tests
    └── OrderRepositoryTests.cs  # Repository implementation tests
```

## Entities

### Order
- `Id`: Unique identifier (GUID, converted to MongoDB string)
- `OrderDate`: Date and time of the order
- `CustomerName`: Name of the customer
- `TotalAmount`: Total amount (calculated from order lines)
- `OrderLines`: Collection of order line items

### OrderLine
- `Id`: Unique identifier (GUID, converted to MongoDB string)
- `ProductName`: Name of the product
- `Quantity`: Quantity ordered
- `UnitPrice`: Price per unit
- `LineTotal`: Total for this line (calculated: Quantity × UnitPrice)
- `OrderId`: Foreign key to the parent order (GUID)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{id:guid}` | Get a specific order by GUID |
| POST | `/api/orders` | Create a new order |
| PUT | `/api/orders/{id:guid}` | Update an existing order |
| DELETE | `/api/orders/{id:guid}` | Delete an order |

## Running the Application

1. **Start MongoDB**: Ensure MongoDB is running on `localhost:27017`

3. **Automatic Data Seeding**: On first startup, the application will automatically seed the database with 5 sample orders containing various products (laptops, phones, accessories, etc.)

2. **Run the application**:
   ```bash
   dotnet run --project MongoDBDriver
   ```

4. **Access the API**: The API will be available at `https://localhost:5201` or `http://localhost:5200`

5. **Test with sample requests**: Use the provided `MongoDBDriver.http` file with your HTTP client

## Sample Usage

### Create an Order
```json
POST /api/orders
{
  "customerName": "John Doe",
  "orderDate": "2024-01-15T10:30:00Z",
  "orderLines": [
    {
      "productName": "Laptop",
      "quantity": 1,
      "unitPrice": 999.99
    },
    {
      "productName": "Mouse",
      "quantity": 2,
      "unitPrice": 25.50
    }
  ]
}
```

### Get All Orders
```
GET /api/orders
```

### Get Specific Order
```
GET /api/orders/550e8400-e29b-41d4-a716-446655440000
```

### Update an Order
```json
PUT /api/orders/550e8400-e29b-41d4-a716-446655440000
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "customerName": "Jane Smith",
  "orderDate": "2024-01-16T14:30:00Z",
  "orderLines": [
    {
      "productName": "Desktop Computer",
      "quantity": 1,
      "unitPrice": 1299.99
    }
  ]
}
```

### Delete an Order
```
DELETE /api/orders/550e8400-e29b-41d4-a716-446655440000
```

## Configuration

The MongoDB connection string can be configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017/OrdersDb"
  }
}
```

## Key Features Demonstrated

1. **Clean Domain Models**: Domain entities are free from infrastructure concerns (no BSON attributes)
2. **Type-Safe Identifiers**: Uses GUID for type safety and better API design
3. **MongoDB EF Core Integration**: Shows how to configure and use MongoDB with Entity Framework Core
4. **GUID to MongoDB Conversion**: Automatic conversion between GUID and MongoDB string storage
5. **Document Relationships**: Demonstrates one-to-many relationships between Orders and OrderLines
6. **CRUD Operations**: Complete Create, Read, Update, Delete functionality
7. **Data Validation**: Model validation using data annotations
8. **Error Handling**: Proper exception handling and HTTP status codes
9. **Logging**: Structured logging for debugging and monitoring
10. **Separation of Concerns**: Infrastructure configuration is separated from domain models

## Architecture Notes

### Clean Domain Models
The domain models (`Order` and `OrderLine`) are kept clean and free from infrastructure concerns:
- No MongoDB BSON attributes in domain classes
- Only business logic and data validation annotations
- GUID identifiers for type safety
- Infrastructure configuration is handled in the `OrderContext`

### GUID to MongoDB Conversion
The application uses GUIDs in the domain models but stores them as strings in MongoDB:
- **Domain Layer**: Uses `Guid` for type safety and better API design
- **Storage Layer**: Converts GUIDs to strings (without hyphens) for MongoDB storage
- **API Layer**: Accepts and returns GUIDs in JSON format
- **Route Constraints**: Uses `{id:guid}` for type-safe routing

### MongoDB Configuration
All MongoDB-specific configuration is handled through:
- **MongoDbSettings**: Configuration class for connection strings and collection names
- **MongoDbService**: Service for MongoDB client and database access
- **Document Models**: BSON-attributed models for MongoDB storage (OrderDocument, OrderLineDocument)
- **Repository Implementation**: Direct MongoDB.Driver queries and operations

### Key Benefits
- **Type Safety**: GUID identifiers prevent common ID-related bugs
- **Testability**: Domain models can be easily unit tested without MongoDB dependencies
- **Maintainability**: Changes to persistence technology don't affect domain models
- **Clean Architecture**: Clear separation between domain and infrastructure layers
- **Performance**: GUIDs provide better performance than string comparisons
- **Flexibility**: Easy to switch persistence providers if needed

## Notes

- The application automatically calculates the `TotalAmount` for orders based on order lines
- Order lines automatically calculate `LineTotal` based on quantity and unit price
- GUIDs are generated automatically for new entities
- GUIDs are converted to strings for MongoDB storage but remain as GUIDs in the domain
- The application includes proper error handling and validation
- Domain models are kept clean of infrastructure concerns following DDD principles
- Route constraints ensure only valid GUIDs are accepted in API endpoints

## Package Versions

This project uses the latest versions of the following packages:

- **MongoDB.Driver**: 3.4.0 (Latest official MongoDB .NET Driver)
- **Microsoft.AspNetCore.OpenApi**: 9.0.6 (Latest)
- **FluentValidation.AspNetCore**: 11.3.1
- **MediatR**: 12.5.0
- **Moq**: 4.20.72 (for testing)
- **.NET**: 9.0

All packages are kept up to date with the latest stable releases for optimal performance and security.

## Sample Data

The application automatically seeds the database with sample orders on first startup:

### Seeded Orders Include:
1. **John Doe** - MacBook Pro 16", Magic Mouse, USB-C Cables
2. **Jane Smith** - Dell XPS 13, Wireless Keyboard  
3. **Bob Johnson** - iPhone 15 Pro (2x), AirPods Pro (2x), iPhone Cases (2x)
4. **Alice Wilson** - Surface Laptop 5, Surface Pen
5. **Charlie Brown** - Gaming Monitor 27", Mechanical Keyboard, Gaming Mouse, Mouse Pad

Each order contains realistic product data with quantities, unit prices, and calculated totals. The seeding only occurs if the database is empty, so it's safe to restart the application without duplicating data.

To reset the data, simply clear your MongoDB database and restart the application.

## Migration from EF Core to MongoDB .NET Driver

This project was successfully migrated from MongoDB Entity Framework Core Provider to the official MongoDB .NET Driver. The migration included:

### What Changed:
- **Dependency**: Replaced `MongoDB.EntityFrameworkCore` with `MongoDB.Driver`
- **Data Access**: Replaced EF Core `DbContext` with direct MongoDB client operations
- **Document Models**: Created BSON-attributed document models for MongoDB storage
- **Repository Implementation**: Rewrote repository using MongoDB.Driver query syntax
- **Configuration**: Updated dependency injection to use MongoDB services

### What Remained the Same:
- **Domain Models**: All domain entities and business logic preserved unchanged
- **API Endpoints**: All REST endpoints work exactly the same
- **Application Layer**: CQRS handlers and DTOs unchanged
- **Business Rules**: All domain validation and business rules preserved
- **Data Format**: MongoDB documents maintain the same structure

### Benefits of Migration:
- **Performance**: Direct MongoDB driver operations are more efficient
- **Features**: Access to full MongoDB feature set and latest updates
- **Control**: Fine-grained control over MongoDB operations and queries
- **Maintenance**: Official MongoDB support and regular updates
- **Flexibility**: Easier to implement complex MongoDB-specific operations

## Testing

Run the comprehensive test suite to validate the migration:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test MongoDBDriver.Tests/

# Run with detailed output
dotnet test --verbosity normal
```

The test suite includes:
- **Domain Model Tests**: Validate all business logic and domain rules
- **Repository Tests**: Ensure MongoDB operations work correctly
- **Value Object Tests**: Verify all validation rules are preserved
- **Integration Tests**: End-to-end functionality validation

## Docker Compose Setup

The project includes a Docker Compose configuration for easy MongoDB setup with Mongo Express for database management. The Docker files are located in the `MongoDBDriver/` project directory for easy access from Rider.

### Services Included:
- **MongoDB Latest (8.0.9+)**: Main database server with authentication
- **Mongo Express Latest**: Web-based MongoDB admin interface

### Quick Start with Docker:

1. **Navigate to the MongoDBDriver directory**:
   ```bash
   cd MongoDBDriver
   ```

2. **Start MongoDB and Mongo Express**:
   ```bash
   docker compose up -d
   ```

3. **Verify services are running**:
   ```bash
   docker compose ps
   ```

4. **Run the application** (from MongoDBDriver directory):
   ```bash
   dotnet run
   ```

5. **Access services**:
   - **API**: http://localhost:5201/api/orders
   - **Mongo Express**: http://localhost:8081 (Database admin interface)

### Using with JetBrains Rider:

1. **Open the solution** in Rider
2. **Right-click on `docker-compose.yml`** in the MongoDBDriver project
3. **Select "Run docker-compose.yml"** or use the Docker tool window
4. **Run the MongoDBDriver project** using Rider's run configuration
5. **Access the API** and Mongo Express from the URLs above3. **Run the application**:
   ```bash
   dotnet run --project MongoDBDriver
   ```

4. **Access services**:
   - **API**: http://localhost:5201/api/orders
   - **Mongo Express**: http://localhost:8081 (Database admin interface)

### Docker Configuration Details:

- **MongoDB**: 
  - Port: 27017
  - Database: OrdersDb
  - User: eftest_user
  - Password: eftest_password
  - Admin: admin/password123

- **Mongo Express**:
  - Port: 8081
  - No authentication required for development

### Stopping Services:
```bash
docker compose down
```

### Resetting Data:
```bash
docker compose down -v  # Removes volumes and data
docker compose up -d    # Restart with fresh database
```

## FluentValidation Implementation

The application uses FluentValidation for comprehensive input validation, keeping domain models clean and providing flexible validation rules.

### Validation Features

- **Clean Domain Models**: No validation attributes in domain classes
- **Comprehensive Rules**: Business logic validation with custom rules
- **Nested Validation**: Automatic validation of embedded OrderLines
- **Custom Validators**: Complex business rules like total amount verification
- **Detailed Error Messages**: Clear, actionable validation error responses

### Validators

#### OrderValidator
- Customer name validation (required, length, character restrictions)
- Order date validation (required, not too far in future)
- Total amount validation (non-negative, reasonable limits)
- Order lines validation (required, count limits)
- Custom rule: Total amount must match sum of order line totals

#### OrderLineValidator
- Product name validation (required, length, character restrictions)
- Quantity validation (positive, reasonable limits)
- Unit price validation (positive, decimal precision, reasonable limits)
- Custom rule: Line total calculation verification

### Example Validation Response

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "CustomerName": [
      "Customer name is required",
      "Customer name must be between 1 and 100 characters"
    ],
    "OrderLines[0].Quantity": [
      "Quantity must be at least 1"
    ],
    "OrderLines[0].UnitPrice": [
      "Unit price must be greater than 0"
    ]
  }
}
```

### Benefits

- **Maintainable**: Validation logic separated from domain models
- **Flexible**: Easy to modify validation rules without changing models
- **Testable**: Validators can be unit tested independently
- **Comprehensive**: Supports complex business rules and cross-field validation
- **User-Friendly**: Detailed error messages for better API experience

## Minimal API Implementation

The application uses .NET's Minimal APIs for a lightweight, modern approach to building HTTP APIs with reduced ceremony and improved performance.

### Minimal API Features

- **Lightweight**: No controller classes, reduced boilerplate code
- **Performance**: Better performance compared to traditional controllers
- **Modern Syntax**: Uses latest C# features and patterns
- **OpenAPI Integration**: Automatic OpenAPI/Swagger documentation generation
- **Dependency Injection**: Full DI support with parameter injection
- **Route Groups**: Organized endpoints with shared configuration

### API Endpoints

All endpoints are defined in `OrderEndpoints.cs` using route groups:

```csharp
var orders = app.MapGroup("/api/orders")
    .WithTags("Orders")
    .WithOpenApi();

orders.MapGet("/", GetAllOrders)
    .WithName("GetAllOrders")
    .WithSummary("Get all orders");

orders.MapPost("/", CreateOrder)
    .WithName("CreateOrder")
    .Accepts<Order>("application/json")
    .Produces<Order>(201);
```

### Benefits of Minimal APIs

- **Reduced Code**: 50% less code compared to traditional controllers
- **Better Performance**: Faster startup and request processing
- **Modern Patterns**: Uses latest .NET features and dependency injection
- **OpenAPI First**: Built-in OpenAPI documentation generation
- **Functional Style**: Clean, functional programming approach
- **Easy Testing**: Simple to unit test individual endpoint functions

### Endpoint Structure

Each endpoint is implemented as a static method with dependency injection:

```csharp
private static async Task<IResult> CreateOrder(
    Order order,                    // Request body
    OrderContext context,           // Injected service
    IValidator<Order> validator,    // Injected service
    ILogger<Program> logger)        // Injected service
{
    // Implementation
}
```

### Validation Integration

FluentValidation is seamlessly integrated with Minimal APIs:

- Automatic model validation
- Custom validation error responses
- Structured error format
- Type-safe validation rules

This implementation demonstrates modern .NET API development patterns while maintaining clean architecture and comprehensive validation.
