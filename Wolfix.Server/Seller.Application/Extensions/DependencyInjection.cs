using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Seller.Application.EventHandlers;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSellerApplicationServices(this IServiceCollection services)
    {
        
        
        return services;
    }

    public static IServiceCollection AddSellerEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<SellerAccountCreated>, SellerAccountCreatedEventHandler>();

        return services;
    }
}