namespace EFTest.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public List<OrderLineDto> OrderLines { get; set; } = new();
}

public class OrderLineDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal LineTotal { get; set; }
}

public class CreateOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public DateTime? OrderDate { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; } = new();
}

public class CreateOrderLineDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
}

public class UpdateOrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<UpdateOrderLineDto> OrderLines { get; set; } = new();
}

public class UpdateOrderLineDto
{
    public Guid? Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
}
