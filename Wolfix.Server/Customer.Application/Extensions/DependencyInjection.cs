using Customer.Application.EventHandlers;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerApplicationServices(this IServiceCollection services)
    {
        //todo:!!!
        
        return services;
    }

    public static IServiceCollection AddCustomerEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CustomerAccountCreated>, CustomerAccountCreatedEventHandler>();

        return services;
    }
}