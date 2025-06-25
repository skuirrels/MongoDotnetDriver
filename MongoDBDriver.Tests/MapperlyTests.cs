using MongoDBDriver.Application.Mappers;
using MongoDBDriver.Domain.Entities;
using MongoDBDriver.Domain.ValueObjects;

namespace MongoDBDriver.Tests;

public class MapperlyTests
{
    private readonly OrderMapper _mapper;

    public MapperlyTests()
    {
        _mapper = new OrderMapper();
    }

    [Fact]
    public void OrderMapper_MapToDto_WithValidOrder_MapsCorrectly()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var order = Order.Create(customerName, DateTime.UtcNow);
        
        order.AddOrderLine(
            ProductName.Create("MacBook Pro"),
            Quantity.Create(1),
            Money.CreateUsd(2499.99m)
        );
        
        order.AddOrderLine(
            ProductName.Create("Magic Mouse"),
            Quantity.Create(2),
            Money.CreateUsd(79.99m)
        );

        // Act
        var dto = _mapper.MapToDto(order);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(order.OrderDate, dto.OrderDate);
        Assert.Equal("John Doe", dto.CustomerName.Value);
        Assert.Equal(2659.97m, dto.TotalAmount.Amount); // 2499.99 + (2 * 79.99)
        Assert.Equal("USD", dto.TotalAmount.Currency);
        Assert.Equal(2, dto.OrderLines.Count);

        // Check first order line
        var firstLine = dto.OrderLines[0];
        Assert.Equal("MacBook Pro", firstLine.ProductName.Value);
        Assert.Equal(1, firstLine.Quantity.Value);
        Assert.Equal(2499.99m, firstLine.UnitPrice.Amount);
        Assert.Equal("USD", firstLine.UnitPrice.Currency);
        Assert.Equal(2499.99m, firstLine.LineTotal.Amount);

        // Check second order line
        var secondLine = dto.OrderLines[1];
        Assert.Equal("Magic Mouse", secondLine.ProductName.Value);
        Assert.Equal(2, secondLine.Quantity.Value);
        Assert.Equal(79.99m, secondLine.UnitPrice.Amount);
        Assert.Equal("USD", secondLine.UnitPrice.Currency);
        Assert.Equal(159.98m, secondLine.LineTotal.Amount); // 2 * 79.99
    }

    [Fact]
    public void OrderMapper_CreateValueObjects_WithValidDtos_CreatesCorrectly()
    {
        // Arrange
        var customerNameDto = new Application.DTOs.CustomerNameDto { Value = "Jane Smith" };
        var productNameDto = new Application.DTOs.ProductNameDto { Value = "iPhone 15" };
        var quantityDto = new Application.DTOs.QuantityDto { Value = 3 };
        var moneyDto = new Application.DTOs.MoneyDto { Amount = 999.99m, Currency = "USD" };

        // Act
        var customerName = _mapper.CreateCustomerName(customerNameDto);
        var productName = _mapper.CreateProductName(productNameDto);
        var quantity = _mapper.CreateQuantity(quantityDto);
        var money = _mapper.CreateMoney(moneyDto);

        // Assert
        Assert.Equal("Jane Smith", customerName.Value);
        Assert.Equal("iPhone 15", productName.Value);
        Assert.Equal(3, quantity.Value);
        Assert.Equal(999.99m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void OrderMapper_MapValueObjectsToDto_WithValidValueObjects_MapsCorrectly()
    {
        // Arrange
        var customerName = CustomerName.Create("Bob Johnson");
        var productName = ProductName.Create("AirPods Pro");
        var quantity = Quantity.Create(5);
        var money = Money.Create(249.99m, "EUR");

        // Act
        var customerNameDto = _mapper.MapToDto(customerName);
        var productNameDto = _mapper.MapToDto(productName);
        var quantityDto = _mapper.MapToDto(quantity);
        var moneyDto = _mapper.MapToDto(money);

        // Assert
        Assert.Equal("Bob Johnson", customerNameDto.Value);
        Assert.Equal("AirPods Pro", productNameDto.Value);
        Assert.Equal(5, quantityDto.Value);
        Assert.Equal(249.99m, moneyDto.Amount);
        Assert.Equal("EUR", moneyDto.Currency);
    }

    [Fact]
    public void OrderMapper_MapMultipleOrders_WithValidOrders_MapsCorrectly()
    {
        // Arrange
        var orders = new List<Order>
        {
            Order.Create(CustomerName.Create("Alice Smith"), DateTime.UtcNow.AddDays(-1)),
            Order.Create(CustomerName.Create("Bob Johnson"), DateTime.UtcNow.AddDays(-2))
        };

        orders[0].AddOrderLine(ProductName.Create("Product A"), Quantity.Create(1), Money.CreateUsd(100m));
        orders[1].AddOrderLine(ProductName.Create("Product B"), Quantity.Create(2), Money.CreateUsd(200m));

        // Act
        var dtos = _mapper.MapToDto(orders).ToList();

        // Assert
        Assert.Equal(2, dtos.Count);
        Assert.Equal("Alice Smith", dtos[0].CustomerName.Value);
        Assert.Equal("Bob Johnson", dtos[1].CustomerName.Value);
        Assert.Equal(100m, dtos[0].TotalAmount.Amount);
        Assert.Equal(400m, dtos[1].TotalAmount.Amount); // 2 * 200
    }
}
