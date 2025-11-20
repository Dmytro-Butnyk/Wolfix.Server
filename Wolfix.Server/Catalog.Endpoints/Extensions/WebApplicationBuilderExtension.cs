using Catalog.Application.Extensions;
using Catalog.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, string connectionString, string toxicApiBaseUrl)
    {
        services.AddCatalogDbContext(connectionString);
        services.AddCatalogRepositories();
        services.AddCatalogInfrastructureServices(toxicApiBaseUrl);
        services.AddCatalogApplicationServices();
        services.AddCatalogEventHandlers();
        
        return services;
    }
}