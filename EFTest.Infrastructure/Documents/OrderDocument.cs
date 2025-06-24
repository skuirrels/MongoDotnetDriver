using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EFTest.Infrastructure.Documents;

public class OrderDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [BsonElement("customerName")]
    public string CustomerName { get; set; } = string.Empty;
    
    [BsonElement("totalAmount")]
    public decimal TotalAmount { get; set; }
    
    [BsonElement("currency")]
    public string Currency { get; set; } = "USD";
    
    [BsonElement("orderLines")]
    public List<OrderLineDocument> OrderLines { get; set; } = new();
}

public class OrderLineDocument
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [BsonElement("productName")]
    public string ProductName { get; set; } = string.Empty;
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    
    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }
    
    [BsonElement("currency")]
    public string Currency { get; set; } = "USD";
    
    [BsonIgnore]
    public decimal LineTotal => Quantity * UnitPrice;
}
