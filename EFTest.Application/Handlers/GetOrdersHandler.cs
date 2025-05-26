using EFTest.Application.DTOs;
using EFTest.Application.Queries;
using EFTest.Application.Services;
using EFTest.Domain.Repositories;
using MediatR;

namespace EFTest.Application.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMappingService _mappingService;

    public GetAllOrdersHandler(
        IOrderRepository orderRepository,
        IOrderMappingService mappingService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(_mappingService.MapToDto);
    }
}

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMappingService _mappingService;

    public GetOrderByIdHandler(
        IOrderRepository orderRepository,
        IOrderMappingService mappingService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        return order != null ? _mappingService.MapToDto(order) : null;
    }
}
