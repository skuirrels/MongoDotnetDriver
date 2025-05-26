namespace EFTest.Domain.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public string CustomerName { get; }
    public decimal TotalAmount { get; }

    public OrderCreatedEvent(Guid orderId, string customerName, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
    }
}
