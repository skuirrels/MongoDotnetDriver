using Microsoft.EntityFrameworkCore;
using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;
using EFTest.Infrastructure.Data;

namespace EFTest.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderContext _context;

    public OrderRepository(OrderContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var orderEntity = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return orderEntity != null ? MapToDomain(orderEntity) : null;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orderEntities = await _context.Orders
            .ToListAsync(cancellationToken);

        return orderEntities.Select(MapToDomain);
    }

    public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
    {
        var orderEntities = await _context.Orders
            .Where(o => o.CustomerName.Contains(customerName))
            .ToListAsync(cancellationToken);

        return orderEntities.Select(MapToDomain);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderEntity = MapToEntity(order);
        await _context.Orders.AddAsync(orderEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderEntity = MapToEntity(order);
        _context.Orders.Update(orderEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderEntity = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        if (orderEntity != null)
        {
            _context.Orders.Remove(orderEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == id, cancellationToken);
    }

    private Order MapToDomain(OrderEntity entity)
    {
        var customerName = CustomerName.Create(entity.CustomerName);
        var order = Order.Create(customerName, entity.OrderDate);

        // Use reflection to set the Id since it's protected
        var idField = typeof(Order).BaseType!.GetField("<Id>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(order, entity.Id);

        // Add order lines
        foreach (var lineEntity in entity.OrderLines)
        {
            var productName = ProductName.Create(lineEntity.ProductName);
            var quantity = Quantity.Create(lineEntity.Quantity);
            var unitPrice = Money.Create(lineEntity.UnitPrice, lineEntity.Currency);
            
            order.AddOrderLine(productName, quantity, unitPrice);
        }

        return order;
    }

    private OrderEntity MapToEntity(Order domain)
    {
        var entity = new OrderEntity
        {
            Id = domain.Id,
            OrderDate = domain.OrderDate,
            CustomerName = domain.CustomerName.Value,
            TotalAmount = domain.TotalAmount.Amount,
            Currency = domain.TotalAmount.Currency,
            OrderLines = domain.OrderLines.Select(ol => new OrderLineEntity
            {
                Id = ol.Id,
                ProductName = ol.ProductName.Value,
                Quantity = ol.Quantity.Value,
                UnitPrice = ol.UnitPrice.Amount,
                Currency = ol.UnitPrice.Currency
            }).ToList()
        };

        return entity;
    }
}
