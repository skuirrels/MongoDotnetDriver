using MongoDB.Driver;
using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Domain.ValueObjects;
using EFTest.Infrastructure.Documents;
using EFTest.Infrastructure.Services;

namespace EFTest.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoDbService _mongoDbService;
    private readonly IMongoCollection<OrderDocument> _ordersCollection;

    public OrderRepository(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
        _ordersCollection = _mongoDbService.Orders;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(o => o.Id, id);
        var orderDocument = await _ordersCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return orderDocument != null ? MapToDomain(orderDocument) : null;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orderDocuments = await _ordersCollection
            .Find(Builders<OrderDocument>.Filter.Empty)
            .ToListAsync(cancellationToken);

        return orderDocuments.Select(MapToDomain);
    }

    public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDocument>.Filter.Regex(o => o.CustomerName,
            new MongoDB.Bson.BsonRegularExpression(customerName, "i"));

        var orderDocuments = await _ordersCollection
            .Find(filter)
            .ToListAsync(cancellationToken);

        return orderDocuments.Select(MapToDomain);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var orderDocument = MapToDocument(order);
        await _ordersCollection.InsertOneAsync(orderDocument, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<OrderDocument>.Filter.Eq(o => o.Id, order.Id);
        var orderDocument = MapToDocument(order);

        await _ordersCollection.ReplaceOneAsync(filter, orderDocument, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<OrderDocument>.Filter.Eq(o => o.Id, order.Id);
        await _ordersCollection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(o => o.Id, id);
        var count = await _ordersCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    private Order MapToDomain(OrderDocument document)
    {
        var customerName = CustomerName.Create(document.CustomerName);
        var order = Order.Create(customerName, document.OrderDate);

        // Use reflection to set the Id since it's protected
        var idField = typeof(Order).BaseType!.GetField("<Id>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(order, document.Id);

        // Add order lines
        foreach (var lineDocument in document.OrderLines)
        {
            var productName = ProductName.Create(lineDocument.ProductName);
            var quantity = Quantity.Create(lineDocument.Quantity);
            var unitPrice = Money.Create(lineDocument.UnitPrice, lineDocument.Currency);

            order.AddOrderLine(productName, quantity, unitPrice);
        }

        return order;
    }

    private OrderDocument MapToDocument(Order domain)
    {
        var document = new OrderDocument
        {
            Id = domain.Id,
            OrderDate = domain.OrderDate,
            CustomerName = domain.CustomerName.Value,
            TotalAmount = domain.TotalAmount.Amount,
            Currency = domain.TotalAmount.Currency,
            OrderLines = domain.OrderLines.Select(ol => new OrderLineDocument
            {
                Id = ol.Id,
                ProductName = ol.ProductName.Value,
                Quantity = ol.Quantity.Value,
                UnitPrice = ol.UnitPrice.Amount,
                Currency = ol.UnitPrice.Currency
            }).ToList()
        };

        return document;
    }
}
