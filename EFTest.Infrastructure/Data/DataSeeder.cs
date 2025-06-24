using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using EFTest.Infrastructure.Documents;
using EFTest.Infrastructure.Services;

namespace EFTest.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IMongoDbService mongoDbService, ILogger logger)
    {
        try
        {
            var ordersCollection = mongoDbService.Orders;

            // Check if data already exists
            var existingCount = await ordersCollection.CountDocumentsAsync(Builders<OrderDocument>.Filter.Empty);
            if (existingCount > 0)
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Seeding database with sample orders...");

            var orders = new List<OrderDocument>
            {
                new OrderDocument
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "John Doe",
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    TotalAmount = 2619.96m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineDocument>
                    {
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "MacBook Pro 16\"",
                            Quantity = 1,
                            UnitPrice = 2499.99m,
                            Currency = "USD"
                        },
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Magic Mouse",
                            Quantity = 1,
                            UnitPrice = 79.99m,
                            Currency = "USD"
                        },
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "USB-C Cable",
                            Quantity = 2,
                            UnitPrice = 19.99m,
                            Currency = "USD"
                        }
                    }
                },
                new OrderDocument
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Jane Smith",
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    TotalAmount = 1389.98m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineDocument>
                    {
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Dell XPS 13",
                            Quantity = 1,
                            UnitPrice = 1299.99m,
                            Currency = "USD"
                        },
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Wireless Keyboard",
                            Quantity = 1,
                            UnitPrice = 89.99m,
                            Currency = "USD"
                        }
                    }
                },
                new OrderDocument
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Bob Johnson",
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    TotalAmount = 2599.94m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineDocument>
                    {
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "iPhone 15 Pro",
                            Quantity = 2,
                            UnitPrice = 999.99m,
                            Currency = "USD"
                        },
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "AirPods Pro",
                            Quantity = 2,
                            UnitPrice = 249.99m,
                            Currency = "USD"
                        },
                        new OrderLineDocument
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "iPhone Case",
                            Quantity = 2,
                            UnitPrice = 49.99m,
                            Currency = "USD"
                        }
                    }
                }
            };

            // Insert orders into MongoDB
            await ordersCollection.InsertManyAsync(orders);

            logger.LogInformation("Successfully seeded {OrderCount} orders with {OrderLineCount} order lines.",
                orders.Count,
                orders.Sum(o => o.OrderLines.Count));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
