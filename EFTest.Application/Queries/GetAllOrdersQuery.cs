using EFTest.Application.DTOs;
using MediatR;

namespace EFTest.Application.Queries;

public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
}

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; set; }
}
