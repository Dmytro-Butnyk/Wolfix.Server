using Microsoft.Extensions.DependencyInjection;
using Order.Application.Interfaces;
using Order.Application.Services;

namespace Order.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        
        return services;
    }
}