using Catalog.IntegrationEvents;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Seller.Application.EventHandlers;
using Seller.Application.Interfaces;
using Seller.Application.Services;
using Seller.Domain.Interfaces.DomainServices;
using Seller.Domain.Services;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSellerApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISellerService, SellerService>();
        services.AddScoped<ISellerApplicationService, SellerApplicationService>();
        
        return services;
    }

    public static IServiceCollection AddSellerEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<SellerAccountCreated, Guid>, SellerAccountCreatedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<GetSellerProfileId, Guid>, GetSellerProfileIdEventHandler>();
        services
            .AddScoped<IIntegrationEventHandler<CheckSellerExistsForProductAddition>,
                CheckSellerExistsForProductAdditionEventHandler>();
        return services;
    }

    public static IServiceCollection AddSellerDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ISellerDomainService, SellerDomainService>();

        return services;
    }
}