using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Support.Domain.Entities;

namespace Support.Infrastructure.MongoDB.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportMongoDb(this IServiceCollection services, string connectionString, string databaseName)
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention() 
        };
        ConventionRegistry.Register("CamelCaseConv", pack, t => true);
    
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
    
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(connectionString));

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
        
        return services;
    }

    public static async Task AddSupportMongoDbIndexes(this IServiceProvider serviceProvider)
    {
        var mongoDb = serviceProvider.GetRequiredService<IMongoDatabase>();
        
        var supportRequestsCollection = mongoDb.GetCollection<SupportRequest>("support_requests");
        
        var indexBuilder = Builders<SupportRequest>.IndexKeys;

        var supportDashboardIndex = new CreateIndexModel<SupportRequest>(
            indexBuilder.Ascending(sr => sr.Status).Descending(sr => sr.CreatedAt),
            new CreateIndexOptions { Name = "idx_status_createdAt_desc" }
        );

        var supportFilterIndex = new CreateIndexModel<SupportRequest>(
            indexBuilder.Ascending(sr => sr.Status).Ascending(sr => sr.Category).Descending(sr => sr.CreatedAt),
            new CreateIndexOptions { Name = "idx_status_category_createdAt_desc" }
        );

        var customerIndex = new CreateIndexModel<SupportRequest>(
            indexBuilder.Ascending(sr => sr.CustomerId).Descending(sr => sr.CreatedAt),
            new CreateIndexOptions { Name = "idx_customerId_createdAt_desc" }
        );

        var customerFilterIndex = new CreateIndexModel<SupportRequest>(
            indexBuilder.Ascending(sr => sr.CustomerId).Ascending(sr => sr.Category).Descending(sr => sr.CreatedAt),
            new CreateIndexOptions { Name = "idx_customerId_category_createdAt_desc" }
        );

        await supportRequestsCollection.Indexes.CreateManyAsync(
            [supportDashboardIndex, supportFilterIndex, customerIndex, customerFilterIndex]
        );
    }
}