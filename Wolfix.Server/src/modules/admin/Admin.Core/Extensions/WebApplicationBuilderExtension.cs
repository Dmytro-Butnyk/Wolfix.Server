using Admin.Infrastructure.Extensions;

namespace Admin.Core.Extensions;

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