using EFTest.Application.Commands;
using EFTest.Application.DTOs;
using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;

namespace EFTest.Application.Mappers;

/// <summary>
/// Mapper interface for Order-related mappings
/// </summary>
public interface IOrderMapper
{
    // Entity to DTO mappings
    OrderDto MapToDto(Order order);
    OrderLineDto MapToDto(OrderLine orderLine);
    IEnumerable<OrderDto> MapToDto(IEnumerable<Order> orders);

    // Value object to DTO mappings
    CustomerNameDto MapToDto(CustomerName customerName);
    MoneyDto MapToDto(Money money);
    ProductNameDto MapToDto(ProductName productName);
    QuantityDto MapToDto(Quantity quantity);

    // Command to DTO mappings (for endpoint parameter mapping)
    UpdateOrderDto MapToDto(UpdateOrderCommand command);
    CreateOrderDto MapToDto(CreateOrderCommand command);

    // Helper methods for creating domain entities from DTOs
    CustomerName CreateCustomerName(CustomerNameDto dto);
    ProductName CreateProductName(ProductNameDto dto);
    Quantity CreateQuantity(QuantityDto dto);
    Money CreateMoney(MoneyDto dto);

    // Backward compatibility methods for simple types
    CustomerName CreateCustomerName(string customerName);
    ProductName CreateProductName(string productName);
    Quantity CreateQuantity(int quantity);
    Money CreateMoney(decimal amount, string currency);
}
