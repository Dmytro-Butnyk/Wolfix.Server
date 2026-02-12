using Microsoft.Extensions.DependencyInjection;
using Order.Application.Extensions;
using Order.Infrastructure.Extensions;

namespace Order.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddOrderModule(this IServiceCollection services, string connectionString, string publishableKey, string secretKey, string webhookKey)
    {
        services.AddOrderDbContext(connectionString);
        services.AddOrderRepositories();
        services.AddOrderStripeOptions(publishableKey, secretKey, webhookKey);
        services.AddOrderInfrastructureServices();

        services.AddOrderApplicationServices();
        services.AddOrderEventHandlers();

        return services;
    }
}