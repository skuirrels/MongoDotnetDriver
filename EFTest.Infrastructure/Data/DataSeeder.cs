using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;
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
            var existingCount = await ordersCollection.CountDocumentsAsync(Builders<Order>.Filter.Empty);
            if (existingCount > 0)
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Seeding database with sample orders...");

            var orders = new List<Order>();

            // Create Order 1: John Doe's tech order
            var order1 = Order.Create(CustomerName.Create("John Doe"), DateTime.UtcNow.AddDays(-5));
            order1.AddOrderLine(
                ProductName.Create("MacBook Pro 16\""),
                Quantity.Create(1),
                Money.CreateUsd(2499.99m)
            );
            order1.AddOrderLine(
                ProductName.Create("Magic Mouse"),
                Quantity.Create(1),
                Money.CreateUsd(79.99m)
            );
            order1.AddOrderLine(
                ProductName.Create("USB-C Cable"),
                Quantity.Create(2),
                Money.CreateUsd(19.99m)
            );
            orders.Add(order1);
            // Create Order 2: Jane Smith's laptop order
            var order2 = Order.Create(CustomerName.Create("Jane Smith"), DateTime.UtcNow.AddDays(-3));
            order2.AddOrderLine(
                ProductName.Create("Dell XPS 13"),
                Quantity.Create(1),
                Money.CreateUsd(1299.99m)
            );
            order2.AddOrderLine(
                ProductName.Create("Wireless Keyboard"),
                Quantity.Create(1),
                Money.CreateUsd(89.99m)
            );
            orders.Add(order2);
            // Create Order 3: Bob Johnson's mobile order
            var order3 = Order.Create(CustomerName.Create("Bob Johnson"), DateTime.UtcNow.AddDays(-1));
            order3.AddOrderLine(
                ProductName.Create("iPhone 15 Pro"),
                Quantity.Create(2),
                Money.CreateUsd(999.99m)
            );
            order3.AddOrderLine(
                ProductName.Create("AirPods Pro"),
                Quantity.Create(2),
                Money.CreateUsd(249.99m)
            );
            order3.AddOrderLine(
                ProductName.Create("iPhone Case"),
                Quantity.Create(2),
                Money.CreateUsd(49.99m)
            );
            orders.Add(order3);

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
