using Catalog.IntegrationEvents;
using Media.Application.EventHandlers;
using Media.Application.Interfaces;
using Media.Application.Options;
using Media.Application.Services;
using Media.IntegrationEvents;
using Media.IntegrationEvents.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seller.IntegrationEvents;
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

    public static IServiceCollection AddMediaServices(this IServiceCollection services)
    {
        services.AddScoped<IBlobResourceService, BlobResourceService>();
        
        return services;
    }
    
    
    public static IServiceCollection AddMediaEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<ProductMediaAdded>, ProductMediaAddedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<ProductMediaDeleted>, ProductMediaDeletedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CategoryAndProductsDeleted>, CategoryAndProductsDeletedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<SellerApplicationCreating, CreatedBlobResourceDto>, SellerApplicationCreatingEventHandler>();
        
        return services;
    }
}