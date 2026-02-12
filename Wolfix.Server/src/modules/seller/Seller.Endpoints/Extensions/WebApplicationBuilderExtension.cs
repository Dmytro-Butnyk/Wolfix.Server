using Microsoft.Extensions.DependencyInjection;
using Seller.Application.Extensions;
using Seller.Infrastructure.Extensions;

namespace Seller.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddSellerModule(this IServiceCollection services, string connectionString)
    {
        services.AddSellerDbContext(connectionString);
        services.AddSellerRepositories();
        services.AddSellerApplicationServices();
        services.AddSellerEventHandlers();
        services.AddSellerDomainServices();

        return services;
    }
}