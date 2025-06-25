using EFTest.Application.DTOs;
using MediatR;

namespace EFTest.Application.Commands;

public class UpdateOrderCommand : IRequest<OrderDto>
{
    public Guid Id { get; set; }
    public CustomerNameDto CustomerName { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public List<UpdateOrderLineDto> OrderLines { get; set; } = new();
}

public class DeleteOrderCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
