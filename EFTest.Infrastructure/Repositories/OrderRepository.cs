using MongoDB.Driver;
using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;
using EFTest.Application.DTOs;
using EFTest.Infrastructure.Services;

namespace EFTest.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoDbService _mongoDbService;
    private readonly IMongoCollection<OrderDto> _ordersCollection;

    public OrderRepository(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
        _ordersCollection = _mongoDbService.Orders;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDto>.Filter.Eq(o => o.Id, id);
        var orderDto = await _ordersCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return orderDto != null ? MapToDomain(orderDto) : null;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orderDtos = await _ordersCollection
            .Find(Builders<OrderDto>.Filter.Empty)
            .ToListAsync(cancellationToken);

        return orderDtos.Select(MapToDomain);
    }

    public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDto>.Filter.Regex(o => o.CustomerName,
            new MongoDB.Bson.BsonRegularExpression(customerName, "i"));

        var orderDtos = await _ordersCollection
            .Find(filter)
            .ToListAsync(cancellationToken);

        return orderDtos.Select(MapToDomain);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderDto = MapToDto(order);
        await _ordersCollection.InsertOneAsync(orderDto, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<OrderDto>.Filter.Eq(o => o.Id, order.Id);
        var orderDto = MapToDto(order);

        await _ordersCollection.ReplaceOneAsync(filter, orderDto, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<OrderDto>.Filter.Eq(o => o.Id, order.Id);
        await _ordersCollection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDto>.Filter.Eq(o => o.Id, id);
        var count = await _ordersCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    private Order MapToDomain(OrderDto dto)
    {
        var customerName = CustomerName.Create(dto.CustomerName);
        var order = Order.Create(customerName, dto.OrderDate);

        // Use reflection to set the Id since it's protected
        var idField = typeof(Order).BaseType!.GetField("<Id>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(order, dto.Id);

        // Add order lines
        foreach (var lineDto in dto.OrderLines)
        {
            var productName = ProductName.Create(lineDto.ProductName);
            var quantity = Quantity.Create(lineDto.Quantity);
            var unitPrice = Money.Create(lineDto.UnitPrice, lineDto.Currency);

            order.AddOrderLine(productName, quantity, unitPrice);
        }

        return order;
    }

    private OrderDto MapToDto(Order domain)
    {
        var dto = new OrderDto
        {
            Id = domain.Id,
            OrderDate = domain.OrderDate,
            CustomerName = domain.CustomerName.Value,
            TotalAmount = domain.TotalAmount.Amount,
            Currency = domain.TotalAmount.Currency,
            OrderLines = domain.OrderLines.Select(ol => new OrderLineDto
            {
                Id = ol.Id,
                ProductName = ol.ProductName.Value,
                Quantity = ol.Quantity.Value,
                UnitPrice = ol.UnitPrice.Amount,
                Currency = ol.UnitPrice.Currency,
                LineTotal = ol.CalculateLineTotal().Amount
            }).ToList()
        };

        return dto;
    }
}
