using Catalog.IntegrationEvents;
using Media.Application.EventHandlers;
using Media.Application.Options;
using Media.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureBlobContainersNames>()
            .Bind(configuration.GetSection("AzureBlobContainersNames"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    
    public static IServiceCollection AddMediaEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<ProductMediaAdded>, ProductMediaAddedEventHandler>();
        
        return services;
    }
}