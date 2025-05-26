using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFTest.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(OrderContext context, ILogger logger)
    {
        try
        {
            // Check if data already exists
            if (await context.Orders.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Seeding database with sample orders...");

            var orders = new List<OrderEntity>
            {
                new OrderEntity
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "John Doe",
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    TotalAmount = 2619.96m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineEntity>
                    {
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "MacBook Pro 16\"",
                            Quantity = 1,
                            UnitPrice = 2499.99m,
                            Currency = "USD"
                        },
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Magic Mouse",
                            Quantity = 1,
                            UnitPrice = 79.99m,
                            Currency = "USD"
                        },
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "USB-C Cable",
                            Quantity = 2,
                            UnitPrice = 19.99m,
                            Currency = "USD"
                        }
                    }
                },
                new OrderEntity
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Jane Smith",
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    TotalAmount = 1389.98m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineEntity>
                    {
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Dell XPS 13",
                            Quantity = 1,
                            UnitPrice = 1299.99m,
                            Currency = "USD"
                        },
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "Wireless Keyboard",
                            Quantity = 1,
                            UnitPrice = 89.99m,
                            Currency = "USD"
                        }
                    }
                },
                new OrderEntity
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Bob Johnson",
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    TotalAmount = 2599.94m,
                    Currency = "USD",
                    OrderLines = new List<OrderLineEntity>
                    {
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "iPhone 15 Pro",
                            Quantity = 2,
                            UnitPrice = 999.99m,
                            Currency = "USD"
                        },
                        new OrderLineEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductName = "AirPods Pro",
                            Quantity = 2,
                            UnitPrice = 249.99m,
                            Currency = "USD"
                        },
                        new OrderLineEntity
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

            // Add orders to context
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

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
