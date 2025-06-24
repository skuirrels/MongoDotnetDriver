using MongoDB.Driver;
using EFTest.Application.DTOs;

namespace EFTest.Infrastructure.Services;

public interface IMongoDbService
{
    IMongoCollection<OrderDto> Orders { get; }
    IMongoDatabase Database { get; }
}
