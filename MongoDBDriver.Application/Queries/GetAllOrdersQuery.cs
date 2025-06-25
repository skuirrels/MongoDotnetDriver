using MongoDBDriver.Application.DTOs;
using MediatR;

namespace MongoDBDriver.Application.Queries;

public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
}

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; set; }
}
