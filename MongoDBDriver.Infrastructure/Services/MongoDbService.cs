using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDBDriver.Infrastructure.Configuration;
using MongoDBDriver.Domain.Entities;

namespace MongoDBDriver.Infrastructure.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    static MongoDbService()
    {
        // Register GUID serializer to handle GUIDs as strings
        BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(GuidRepresentation.Standard));

        // Register conventions for cleaner field names
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new IgnoreIfDefaultConvention(true)
        };
        ConventionRegistry.Register("camelCase", conventionPack, t => true);
    }

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;

        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Order> Orders =>
        _database.GetCollection<Order>(_settings.OrdersCollectionName);

    public IMongoDatabase Database => _database;
}
