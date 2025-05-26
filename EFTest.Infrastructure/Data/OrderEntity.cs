namespace EFTest.Infrastructure.Data;

// Simplified entity for MongoDB persistence
public class OrderEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public List<OrderLineEntity> OrderLines { get; set; } = new();
}

public class OrderLineEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal LineTotal => Quantity * UnitPrice;
}
