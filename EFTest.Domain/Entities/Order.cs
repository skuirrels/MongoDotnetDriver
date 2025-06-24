using EFTest.Domain.Events;
using EFTest.Domain.ValueObjects;

namespace EFTest.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderLine> _orderLines = new();

    public DateTime OrderDate { get; private set; }
    public CustomerName CustomerName { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();

    private Order(CustomerName customerName, DateTime orderDate)
    {
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        OrderDate = orderDate;
        TotalAmount = Money.ZeroUsd();
        
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerName.Value, TotalAmount.Amount));
    }

    public static Order Create(CustomerName customerName, DateTime? orderDate = null)
    {
        var date = orderDate ?? DateTime.UtcNow;
        
        if (date > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Order date cannot be more than 1 day in the future", nameof(orderDate));

        return new Order(customerName, date);
    }

    public void AddOrderLine(ProductName productName, Quantity quantity, Money unitPrice)
    {
        if (productName == null) throw new ArgumentNullException(nameof(productName));
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        if (unitPrice == null) throw new ArgumentNullException(nameof(unitPrice));

        if (_orderLines.Count >= 50)
            throw new InvalidOperationException("Order cannot contain more than 50 order lines");

        var orderLine = OrderLine.Create(productName, quantity, unitPrice);
        _orderLines.Add(orderLine);
        
        RecalculateTotalAmount();
    }

    public void RemoveOrderLine(Guid orderLineId)
    {
        var orderLine = _orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
            throw new ArgumentException($"Order line with ID {orderLineId} not found", nameof(orderLineId));

        _orderLines.Remove(orderLine);
        RecalculateTotalAmount();
    }

    public void UpdateOrderLine(Guid orderLineId, Quantity? newQuantity = null, Money? newUnitPrice = null)
    {
        var orderLine = _orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
            throw new ArgumentException($"Order line with ID {orderLineId} not found", nameof(orderLineId));

        if (newQuantity != null)
            orderLine.UpdateQuantity(newQuantity);

        if (newUnitPrice != null)
            orderLine.UpdateUnitPrice(newUnitPrice);

        RecalculateTotalAmount();
    }

    public void UpdateCustomerName(CustomerName newCustomerName)
    {
        if (newCustomerName == null)
            throw new ArgumentNullException(nameof(newCustomerName));

        CustomerName = newCustomerName;
    }

    public void UpdateOrderDate(DateTime newOrderDate)
    {
        if (newOrderDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Order date cannot be more than 1 day in the future", nameof(newOrderDate));

        OrderDate = newOrderDate;
    }

    private void RecalculateTotalAmount()
    {
        var previousTotal = TotalAmount;
        
        if (!_orderLines.Any())
        {
            TotalAmount = Money.ZeroUsd();
        }
        else
        {
            var currency = _orderLines.First().UnitPrice.Currency;
            var total = Money.Zero(currency);
            
            foreach (var orderLine in _orderLines)
            {
                total = total.Add(orderLine.CalculateLineTotal());
            }
            
            TotalAmount = total;
        }

        if (previousTotal != null && previousTotal.Amount != TotalAmount.Amount)
        {
            AddDomainEvent(new OrderUpdatedEvent(Id, previousTotal.Amount, TotalAmount.Amount));
        }
    }

    public void ValidateInvariants()
    {
        if (!_orderLines.Any())
            throw new InvalidOperationException("Order must contain at least one order line");

        if (TotalAmount.Amount >= 1000000)
            throw new InvalidOperationException("Order total cannot exceed $1,000,000");
    }
}
