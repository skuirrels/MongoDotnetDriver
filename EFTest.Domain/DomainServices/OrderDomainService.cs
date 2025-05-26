using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;

namespace EFTest.Domain.DomainServices;

public class OrderDomainService : IOrderDomainService
{
    private readonly IOrderRepository _orderRepository;

    public OrderDomainService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<bool> CanCreateOrderAsync(CustomerName customerName, CancellationToken cancellationToken = default)
    {
        // Business rule: Check if customer has any pending orders
        var existingOrders = await _orderRepository.GetByCustomerNameAsync(customerName.Value, cancellationToken);
        
        // For demo purposes, allow unlimited orders
        // In real scenario, might check credit limits, pending orders, etc.
        return true;
    }

    public async Task<Money> CalculateOrderDiscountAsync(Order order, CancellationToken cancellationToken = default)
    {
        // Business rule: Apply discount based on order total
        var totalAmount = order.TotalAmount;
        
        if (totalAmount.Amount >= 1000)
        {
            // 10% discount for orders over $1000
            return Money.Create(totalAmount.Amount * 0.1m, totalAmount.Currency);
        }
        
        if (totalAmount.Amount >= 500)
        {
            // 5% discount for orders over $500
            return Money.Create(totalAmount.Amount * 0.05m, totalAmount.Currency);
        }

        return Money.Zero(totalAmount.Currency);
    }

    public async Task ValidateOrderBusinessRulesAsync(Order order, CancellationToken cancellationToken = default)
    {
        // Validate domain invariants
        order.ValidateInvariants();

        // Additional business rules
        if (!await CanCreateOrderAsync(order.CustomerName, cancellationToken))
        {
            throw new InvalidOperationException($"Cannot create order for customer {order.CustomerName}");
        }

        // Business rule: Validate order lines have reasonable prices
        foreach (var orderLine in order.OrderLines)
        {
            if (orderLine.UnitPrice.Amount > 100000)
            {
                throw new InvalidOperationException($"Unit price {orderLine.UnitPrice} exceeds maximum allowed amount");
            }
        }
    }
}
