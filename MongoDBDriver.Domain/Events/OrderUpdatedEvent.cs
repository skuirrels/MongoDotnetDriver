namespace MongoDBDriver.Domain.Events;

public class OrderUpdatedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public decimal PreviousTotalAmount { get; }
    public decimal NewTotalAmount { get; }

    public OrderUpdatedEvent(Guid orderId, decimal previousTotalAmount, decimal newTotalAmount)
    {
        OrderId = orderId;
        PreviousTotalAmount = previousTotalAmount;
        NewTotalAmount = newTotalAmount;
    }
}
