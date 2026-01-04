using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Support.Infrastructure.MongoDB.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportMongoDB(this IServiceCollection services, string connectionString, string databaseName)
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
    
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });
        
        return services;
    }
}