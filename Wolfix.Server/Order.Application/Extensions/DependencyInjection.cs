using Microsoft.Extensions.DependencyInjection;
using Order.Application.Services;

namespace Order.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        services.AddScoped<DeliveryMethodService>();
        
        return services;
    }
}