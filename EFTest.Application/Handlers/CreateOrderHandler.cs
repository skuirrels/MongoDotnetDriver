using EFTest.Application.Commands;
using EFTest.Application.DTOs;
using EFTest.Application.Mappers;
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
    private readonly IOrderMapper _mapper;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IOrderDomainService orderDomainService,
        IOrderMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderDomainService = orderDomainService ?? throw new ArgumentNullException(nameof(orderDomainService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create value objects using mapper helper methods with structured DTOs
        var customerName = _mapper.CreateCustomerName(request.CustomerName);

        // Create order entity
        var order = Order.Create(customerName, request.OrderDate);

        // Add order lines using mapper helper methods with structured DTOs
        foreach (var lineDto in request.OrderLines)
        {
            var productName = _mapper.CreateProductName(lineDto.ProductName);
            var quantity = _mapper.CreateQuantity(lineDto.Quantity);
            var unitPrice = _mapper.CreateMoney(lineDto.UnitPrice);

            order.AddOrderLine(productName, quantity, unitPrice);
        }

        // Validate business rules
        await _orderDomainService.ValidateOrderBusinessRulesAsync(order, cancellationToken);

        // Save to repository
        await _orderRepository.AddAsync(order, cancellationToken);

        // Map to structured DTO and return
        return _mapper.MapToDto(order);
    }
}
