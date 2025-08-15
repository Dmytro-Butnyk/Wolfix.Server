using Media.Application.Extensions;
using Media.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Api;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddMediaModule(this IServiceCollection services, string connectionString,
        IConfiguration configuration)
    {
        services.AddMediaDbContext(connectionString);
        services.AddMediaRepositories();
        services.AddAzureBlobRepository();

        services.AddMediaOptions(configuration);
        
        return services;
    }
}