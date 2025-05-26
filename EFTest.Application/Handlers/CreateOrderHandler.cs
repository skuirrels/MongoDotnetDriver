using EFTest.Application.Commands;
using EFTest.Application.DTOs;
using EFTest.Application.Services;
using EFTest.Domain.DomainServices;
using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;
using MediatR;

namespace EFTest.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDomainService _orderDomainService;
    private readonly IOrderMappingService _mappingService;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IOrderDomainService orderDomainService,
        IOrderMappingService mappingService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderDomainService = orderDomainService ?? throw new ArgumentNullException(nameof(orderDomainService));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create value objects
        var customerName = CustomerName.Create(request.CustomerName);
        
        // Create order entity
        var order = Order.Create(customerName, request.OrderDate);

        // Add order lines
        foreach (var lineDto in request.OrderLines)
        {
            var productName = ProductName.Create(lineDto.ProductName);
            var quantity = Quantity.Create(lineDto.Quantity);
            var unitPrice = Money.Create(lineDto.UnitPrice, lineDto.Currency);
            
            order.AddOrderLine(productName, quantity, unitPrice);
        }

        // Validate business rules
        await _orderDomainService.ValidateOrderBusinessRulesAsync(order, cancellationToken);

        // Save to repository
        await _orderRepository.AddAsync(order, cancellationToken);

        // Map to DTO and return
        return _mappingService.MapToDto(order);
    }
}
