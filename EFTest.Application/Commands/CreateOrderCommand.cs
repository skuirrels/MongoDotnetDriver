using EFTest.Application.DTOs;
using MediatR;

namespace EFTest.Application.Commands;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public CustomerNameDto CustomerName { get; set; } = new();
    public DateTime? OrderDate { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; } = new();
}
