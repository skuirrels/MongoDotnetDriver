using MongoDB.Driver;
using EFTest.Domain.Entities;
using EFTest.Domain.Repositories;
using EFTest.Infrastructure.Services;

namespace EFTest.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoDbService _mongoDbService;
    private readonly IMongoCollection<Order> _ordersCollection;

    public OrderRepository(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
        _ordersCollection = _mongoDbService.Orders;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var order = await _ordersCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return order;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _ordersCollection
            .Find(Builders<Order>.Filter.Empty)
            .ToListAsync(cancellationToken);

        return orders;
    }

    public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Regex(o => o.CustomerName.Value,
            new MongoDB.Bson.BsonRegularExpression(customerName, "i"));

        var orders = await _ordersCollection
            .Find(filter)
            .ToListAsync(cancellationToken);

        return orders;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        await _ordersCollection.InsertOneAsync(order, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);

        await _ordersCollection.ReplaceOneAsync(filter, order, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);
        await _ordersCollection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var count = await _ordersCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}
