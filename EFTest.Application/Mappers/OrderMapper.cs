using EFTest.Application.Commands;
using EFTest.Application.DTOs;
using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace EFTest.Application.Mappers;

/// <summary>
/// Mapperly mapper implementation for Order-related mappings
/// Handles complex value object mappings and domain entity conversions
/// Maps between rich domain entities and structured DTOs that mirror the domain model
/// </summary>
[Mapper]
public partial class OrderMapper : IOrderMapper
{
    // Entity to DTO mappings - now mapping to structured DTOs

    [MapperIgnoreSource(nameof(Order.DomainEvents))]
    public partial OrderDto MapToDto(Order order);

    [MapperIgnoreSource(nameof(OrderLine.DomainEvents))]
    [MapProperty(nameof(OrderLine), nameof(OrderLineDto.LineTotal), Use = nameof(MapLineTotal))]
    public partial OrderLineDto MapToDto(OrderLine orderLine);

    // Value object to DTO mappings
    public partial CustomerNameDto MapToDto(CustomerName customerName);
    public partial MoneyDto MapToDto(Money money);
    public partial ProductNameDto MapToDto(ProductName productName);
    public partial QuantityDto MapToDto(Quantity quantity);

    // Custom mapping for LineTotal calculation
    private MoneyDto MapLineTotal(OrderLine orderLine) => MapToDto(orderLine.CalculateLineTotal());

    public partial IEnumerable<OrderDto> MapToDto(IEnumerable<Order> orders);

    // Command to DTO mappings
    public partial UpdateOrderDto MapToDto(UpdateOrderCommand command);
    public partial CreateOrderDto MapToDto(CreateOrderCommand command);



    // Helper methods for creating domain entities from DTOs (used in handlers)

    /// <summary>
    /// Creates CustomerName value object from DTO
    /// </summary>
    public CustomerName CreateCustomerName(CustomerNameDto dto) => CustomerName.Create(dto.Value);

    /// <summary>
    /// Creates ProductName value object from DTO
    /// </summary>
    public ProductName CreateProductName(ProductNameDto dto) => ProductName.Create(dto.Value);

    /// <summary>
    /// Creates Quantity value object from DTO
    /// </summary>
    public Quantity CreateQuantity(QuantityDto dto) => Quantity.Create(dto.Value);

    /// <summary>
    /// Creates Money value object from DTO
    /// </summary>
    public Money CreateMoney(MoneyDto dto) => Money.Create(dto.Amount, dto.Currency);

    // Backward compatibility methods for simple types (if needed)
    /// <summary>
    /// Creates CustomerName value object from string (backward compatibility)
    /// </summary>
    public CustomerName CreateCustomerName(string customerName) => CustomerName.Create(customerName);

    /// <summary>
    /// Creates ProductName value object from string (backward compatibility)
    /// </summary>
    public ProductName CreateProductName(string productName) => ProductName.Create(productName);

    /// <summary>
    /// Creates Quantity value object from int (backward compatibility)
    /// </summary>
    public Quantity CreateQuantity(int quantity) => Quantity.Create(quantity);

    /// <summary>
    /// Creates Money value object from amount and currency (backward compatibility)
    /// </summary>
    public Money CreateMoney(decimal amount, string currency) => Money.Create(amount, currency);
}
