using Admin.Application.Extensions;
using Admin.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddAdminModule(this IServiceCollection services, string connectionString)
    {
        services.AddAdminDbContext(connectionString);
        services.AddAdminRepositories();

        services.AddAdminApplicationServices();
        services.AddAdminEventHandlers();

        return services;
    }
}