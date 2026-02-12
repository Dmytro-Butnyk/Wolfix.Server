using Customer.Application.Extensions;
using Customer.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddCustomerModule(this IServiceCollection services, string connectionString)
    {
        services.AddCustomerDbContext(connectionString);
        services.AddCustomerRepositories();
        services.AddCustomerApplicationServices();
        services.AddCustomerEventHandlers();
        
        return services;
    }
}