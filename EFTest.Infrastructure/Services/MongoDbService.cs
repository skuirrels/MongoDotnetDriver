using Microsoft.Extensions.Options;
using MongoDB.Driver;
using EFTest.Infrastructure.Configuration;
using EFTest.Application.DTOs;

namespace EFTest.Infrastructure.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;

        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<OrderDto> Orders =>
        _database.GetCollection<OrderDto>(_settings.OrdersCollectionName);

    public IMongoDatabase Database => _database;
}
