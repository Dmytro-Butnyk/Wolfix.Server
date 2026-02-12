using Microsoft.Extensions.DependencyInjection;
using Order.Application.EventHandlers;
using Order.Application.Services;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Order.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        services.AddScoped<DeliveryMethodService>();
        
        return services;
    }

    public static IServiceCollection AddOrderEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CheckCustomerOrder>, CheckCustomerOrderEventHandler>();

        return services;
    }
}