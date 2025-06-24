using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;

namespace EFTest.Tests;

public class DomainModelTests
{
    [Fact]
    public void Order_Create_WithValidData_CreatesSuccessfully()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var orderDate = DateTime.UtcNow;

        // Act
        var order = Order.Create(customerName, orderDate);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(customerName, order.CustomerName);
        Assert.Equal(orderDate, order.OrderDate);
        Assert.Equal(0, order.TotalAmount.Amount);
        Assert.Equal("USD", order.TotalAmount.Currency);
        Assert.Empty(order.OrderLines);
    }

    [Fact]
    public void Order_AddOrderLine_WithValidData_AddsSuccessfully()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var order = Order.Create(customerName);
        var productName = ProductName.Create("MacBook Pro");
        var quantity = Quantity.Create(1);
        var unitPrice = Money.CreateUsd(2499.99m);

        // Act
        order.AddOrderLine(productName, quantity, unitPrice);

        // Assert
        Assert.Single(order.OrderLines);
        Assert.Equal(2499.99m, order.TotalAmount.Amount);
        
        var orderLine = order.OrderLines.First();
        Assert.Equal(productName, orderLine.ProductName);
        Assert.Equal(quantity, orderLine.Quantity);
        Assert.Equal(unitPrice, orderLine.UnitPrice);
    }

    [Fact]
    public void Order_AddMultipleOrderLines_CalculatesTotalCorrectly()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var order = Order.Create(customerName);

        // Act
        order.AddOrderLine(ProductName.Create("MacBook Pro"), Quantity.Create(1), Money.CreateUsd(2499.99m));
        order.AddOrderLine(ProductName.Create("Magic Mouse"), Quantity.Create(1), Money.CreateUsd(79.99m));
        order.AddOrderLine(ProductName.Create("USB-C Cable"), Quantity.Create(2), Money.CreateUsd(19.99m));

        // Assert
        Assert.Equal(3, order.OrderLines.Count);
        Assert.Equal(2619.96m, order.TotalAmount.Amount); // 2499.99 + 79.99 + (2 * 19.99)
    }

    [Fact]
    public void CustomerName_Create_WithValidName_CreatesSuccessfully()
    {
        // Arrange
        var name = "John Doe";

        // Act
        var customerName = CustomerName.Create(name);

        // Assert
        Assert.Equal(name, customerName.Value);
    }

    [Fact]
    public void CustomerName_Create_WithInvalidName_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CustomerName.Create(""));
        Assert.Throws<ArgumentException>(() => CustomerName.Create("   "));
        Assert.Throws<ArgumentException>(() => CustomerName.Create("John123")); // Contains numbers
    }

    [Fact]
    public void ProductName_Create_WithValidName_CreatesSuccessfully()
    {
        // Arrange
        var name = "MacBook Pro 16\"";

        // Act
        var productName = ProductName.Create(name);

        // Assert
        Assert.Equal(name, productName.Value);
    }

    [Fact]
    public void Quantity_Create_WithValidQuantity_CreatesSuccessfully()
    {
        // Arrange
        var value = 5;

        // Act
        var quantity = Quantity.Create(value);

        // Assert
        Assert.Equal(value, quantity.Value);
    }

    [Fact]
    public void Quantity_Create_WithInvalidQuantity_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Quantity.Create(0));
        Assert.Throws<ArgumentException>(() => Quantity.Create(-1));
        Assert.Throws<ArgumentException>(() => Quantity.Create(1001)); // Exceeds max
    }

    [Fact]
    public void Money_Create_WithValidAmount_CreatesSuccessfully()
    {
        // Arrange
        var amount = 99.99m;
        var currency = "USD";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Fact]
    public void Money_Create_WithNegativeAmount_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(-1m, "USD"));
    }

    [Fact]
    public void Order_ValidateInvariants_WithNoOrderLines_ThrowsException()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var order = Order.Create(customerName);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.ValidateInvariants());
    }

    [Fact]
    public void Order_AddOrderLine_ExceedingMaxLines_ThrowsException()
    {
        // Arrange
        var customerName = CustomerName.Create("John Doe");
        var order = Order.Create(customerName);
        var productName = ProductName.Create("Test Product");
        var quantity = Quantity.Create(1);
        var unitPrice = Money.CreateUsd(10m);

        // Add 50 order lines (the maximum)
        for (int i = 0; i < 50; i++)
        {
            order.AddOrderLine(productName, quantity, unitPrice);
        }

        // Act & Assert - Adding the 51st line should throw
        Assert.Throws<InvalidOperationException>(() => order.AddOrderLine(productName, quantity, unitPrice));
    }
}
