using EFTest.Application.DTOs;
using EFTest.Domain.Entities;

namespace EFTest.Application.Services;

public class OrderMappingService : IOrderMappingService
{
    public OrderDto MapToDto(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerName = order.CustomerName.Value,
            TotalAmount = order.TotalAmount.Amount,
            Currency = order.TotalAmount.Currency,
            OrderLines = order.OrderLines.Select(MapToDto).ToList()
        };
    }

    public OrderLineDto MapToDto(OrderLine orderLine)
    {
        if (orderLine == null) throw new ArgumentNullException(nameof(orderLine));

        var lineTotal = orderLine.CalculateLineTotal();

        return new OrderLineDto
        {
            Id = orderLine.Id,
            ProductName = orderLine.ProductName.Value,
            Quantity = orderLine.Quantity.Value,
            UnitPrice = orderLine.UnitPrice.Amount,
            Currency = orderLine.UnitPrice.Currency,
            LineTotal = lineTotal.Amount
        };
    }
}
