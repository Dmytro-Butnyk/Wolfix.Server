using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Contracts;
using Order.Application.Models;
using Order.Domain.Interfaces.Order;
using Order.Infrastructure.Options;
using Order.Infrastructure.Repositories;
using Order.Infrastructure.Services;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Order.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static async Task EnsureOrderSchemeExistAndMigrateAsync(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<OrderContext>();

        await db.Database.MigrateAsync();
    }

    public static IServiceCollection AddOrderDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }

    public static IServiceCollection AddOrderRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Domain.OrderAggregate.Order>, BaseRepository<OrderContext, Domain.OrderAggregate.Order>>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    public static IServiceCollection AddOrderStripeOptions(this IServiceCollection services, string publishableKey, string secretKey, string webhookKey)
    {
        services.Configure<StripeOptions>(options =>
        {
            options.PublishableKey = publishableKey;
            options.SecretKey = secretKey;
            options.WebhookKey = webhookKey;
        });

        return services;
    }

    public static IServiceCollection AddOrderInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService<StripePaymentResponse>, StripePaymentService>();

        return services;
    }
}