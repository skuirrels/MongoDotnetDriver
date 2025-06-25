using MongoDB.Driver;
using MongoDBDriver.Domain.Entities;

namespace MongoDBDriver.Infrastructure.Services;

public interface IMongoDbService
{
    IMongoCollection<Order> Orders { get; }
    IMongoDatabase Database { get; }
}
