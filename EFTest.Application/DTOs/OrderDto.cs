namespace EFTest.Application.DTOs;

/// <summary>
/// DTO representing a customer name - mirrors the CustomerName value object
/// </summary>
public class CustomerNameDto
{
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// DTO representing money - mirrors the Money value object
/// </summary>
public class MoneyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// DTO representing a product name - mirrors the ProductName value object
/// </summary>
public class ProductNameDto
{
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// DTO representing quantity - mirrors the Quantity value object
/// </summary>
public class QuantityDto
{
    public int Value { get; set; }
}

/// <summary>
/// DTO representing an order - mirrors the Order entity structure
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public CustomerNameDto CustomerName { get; set; } = new();
    public MoneyDto TotalAmount { get; set; } = new();
    public List<OrderLineDto> OrderLines { get; set; } = new();
}

/// <summary>
/// DTO representing an order line - mirrors the OrderLine entity structure
/// </summary>
public class OrderLineDto
{
    public Guid Id { get; set; }
    public ProductNameDto ProductName { get; set; } = new();
    public QuantityDto Quantity { get; set; } = new();
    public MoneyDto UnitPrice { get; set; } = new();
    public MoneyDto LineTotal { get; set; } = new();
}

/// <summary>
/// DTO for creating a new order - structured like the domain but for input
/// </summary>
public class CreateOrderDto
{
    public CustomerNameDto CustomerName { get; set; } = new();
    public DateTime? OrderDate { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; } = new();
}

/// <summary>
/// DTO for creating a new order line - structured like the domain but for input
/// </summary>
public class CreateOrderLineDto
{
    public ProductNameDto ProductName { get; set; } = new();
    public QuantityDto Quantity { get; set; } = new();
    public MoneyDto UnitPrice { get; set; } = new();
}

/// <summary>
/// DTO for updating an existing order - structured like the domain but for input
/// </summary>
public class UpdateOrderDto
{
    public Guid Id { get; set; }
    public CustomerNameDto CustomerName { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public List<UpdateOrderLineDto> OrderLines { get; set; } = new();
}

/// <summary>
/// DTO for updating an order line - structured like the domain but for input
/// </summary>
public class UpdateOrderLineDto
{
    public Guid? Id { get; set; }
    public ProductNameDto ProductName { get; set; } = new();
    public QuantityDto Quantity { get; set; } = new();
    public MoneyDto UnitPrice { get; set; } = new();
}
