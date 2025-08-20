using Catalog.IntegrationEvents;
using Customer.Application.EventHandlers;
using Customer.Application.Interfaces;
using Customer.Application.Services;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        
        return services;
    }

    public static IServiceCollection AddCustomerEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CustomerAccountCreated>, CustomerAccountCreatedEventHandler>();
        services
            .AddScoped<IIntegrationEventHandler<ProductExistsForAddingToFavorite>,
                ProductExistsForAddingToFavoriteEventHandler>();

        return services;
    }
}