using MongoDB.Driver;
using EFTest.Domain.Entities;

namespace EFTest.Infrastructure.Services;

public interface IMongoDbService
{
    IMongoCollection<Order> Orders { get; }
    IMongoDatabase Database { get; }
}
