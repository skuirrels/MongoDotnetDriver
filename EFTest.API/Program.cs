using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using EFTest.Domain.DomainServices;
using EFTest.Domain.Repositories;
using EFTest.Application.Services;
using EFTest.Application.Validators;
using EFTest.Infrastructure.Data;
using EFTest.Infrastructure.Repositories;
using EFTest.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Add Swagger/OpenAPI services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EFTest Orders API - DDD Demo",
        Version = "v1",
        Description = @"
A comprehensive Domain Driven Design (DDD) demonstration API for managing orders using:

**Architecture:**
- Domain Driven Design (DDD) with rich domain models
- Clean Architecture with proper separation of concerns
- CQRS pattern using MediatR
- Repository pattern with dependency inversion

**Technology Stack:**
- .NET 9.0 with Minimal APIs
- MongoDB with Entity Framework Core
- FluentValidation for comprehensive validation
- Docker Compose for development environment

**Features:**
- Rich domain entities with business logic
- Value objects (Money, CustomerName, ProductName, Quantity)
- Domain events for future event-driven architecture
- Embedded documents following MongoDB best practices
- Comprehensive validation and error handling

**Domain Concepts:**
- Orders contain multiple OrderLines
- Each OrderLine has quantity, unit price, and calculated line total
- Orders automatically calculate total amount from order lines
- Business rules enforced through domain entities and services",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "EFTest DDD Demo",
            Email = "demo@eftest.com",
            Url = new Uri("https://github.com/eftest/ddd-demo")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Add tags for better organization
    c.TagActionsBy(api => new[] { "Orders" });
});

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(EFTest.Application.Commands.CreateOrderCommand).Assembly);
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

// Configure MongoDB Entity Framework
builder.Services.AddDbContext<OrderContext>(options =>
    options.UseMongoDB(builder.Configuration.GetConnectionString("MongoDb")!, "OrdersDb"));

// Register Domain Services
builder.Services.AddScoped<IOrderDomainService, OrderDomainService>();

// Register Application Services
builder.Services.AddScoped<IOrderMappingService, OrderMappingService>();

// Register Infrastructure Services (Repositories)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await DataSeeder.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFTest Orders API v1");
        c.RoutePrefix = "swagger"; // Swagger UI at /swagger
        c.DocumentTitle = "EFTest Orders API - DDD Demo";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        
        // Custom CSS for better appearance
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();

// Serve custom CSS for Swagger
app.UseStaticFiles();

// Map Minimal API endpoints
app.MapOrderEndpoints();

// Add a root endpoint that redirects to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.Run();
