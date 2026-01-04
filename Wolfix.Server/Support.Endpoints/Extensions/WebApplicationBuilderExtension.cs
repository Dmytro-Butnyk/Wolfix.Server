using Microsoft.Extensions.DependencyInjection;
using Support.Application.Extensions;
using Support.Infrastructure.Extensions;
using Support.Infrastructure.MongoDB.Extensions;

namespace Support.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddSupportModule(this IServiceCollection services, string connectionString, string databaseName)
    {
        services.AddSupportDbContext(connectionString);
        services.AddSupportRepositories();
        services.AddSupportApplicationServices();
        services.AddSupportEventHandlers();
        services.AddSupportMongoDB(connectionString, databaseName);

        return services;
    }
}