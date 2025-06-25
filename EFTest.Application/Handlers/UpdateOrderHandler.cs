using EFTest.Application.Commands;
using EFTest.Application.DTOs;
using EFTest.Application.Mappers;
using EFTest.Domain.DomainServices;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;
using MediatR;

namespace EFTest.Application.Handlers;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDomainService _orderDomainService;
    private readonly IOrderMapper _mapper;

    public UpdateOrderHandler(
        IOrderRepository orderRepository,
        IOrderDomainService orderDomainService,
        IOrderMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderDomainService = orderDomainService ?? throw new ArgumentNullException(nameof(orderDomainService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        // Get existing order
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {request.Id} not found");

        // Update customer name if changed using mapper with structured DTOs
        var newCustomerName = _mapper.CreateCustomerName(request.CustomerName);
        if (order.CustomerName.Value != newCustomerName.Value)
        {
            order.UpdateCustomerName(newCustomerName);
        }

        // Update order date if changed
        if (order.OrderDate != request.OrderDate)
        {
            order.UpdateOrderDate(request.OrderDate);
        }

        // Clear existing order lines and add new ones
        // Note: In a real scenario, you might want to update existing lines instead of replacing all
        var existingLineIds = order.OrderLines.Select(ol => ol.Id).ToList();
        foreach (var lineId in existingLineIds)
        {
            order.RemoveOrderLine(lineId);
        }

        // Add new order lines using mapper helper methods with structured DTOs
        foreach (var lineDto in request.OrderLines)
        {
            var productName = _mapper.CreateProductName(lineDto.ProductName);
            var quantity = _mapper.CreateQuantity(lineDto.Quantity);
            var unitPrice = _mapper.CreateMoney(lineDto.UnitPrice);

            order.AddOrderLine(productName, quantity, unitPrice);
        }

        // Validate business rules
        await _orderDomainService.ValidateOrderBusinessRulesAsync(order, cancellationToken);

        // Update in repository
        await _orderRepository.UpdateAsync(order, cancellationToken);

        // Map to DTO and return
        return _mapper.MapToDto(order);
    }
}

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order == null)
            return false;

        await _orderRepository.DeleteAsync(order, cancellationToken);
        return true;
    }
}
