using Microsoft.Extensions.DependencyInjection;
using Support.Application.Extensions;
using Support.Infrastructure.Extensions;

namespace Support.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddSupportModule(this IServiceCollection services, string connectionString)
    {
        services.AddSupportDbContext(connectionString);
        services.AddSupportRepositories();
        services.AddSupportApplicationServices();
        services.AddSupportEventHandlers();

        return services;
    }
}