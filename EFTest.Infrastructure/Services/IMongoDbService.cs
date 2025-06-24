using MongoDB.Driver;
using EFTest.Infrastructure.Documents;

namespace EFTest.Infrastructure.Services;

public interface IMongoDbService
{
    IMongoCollection<OrderDocument> Orders { get; }
    IMongoDatabase Database { get; }
}
