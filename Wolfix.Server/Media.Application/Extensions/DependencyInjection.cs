using Media.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AzureBlobContainersNames>()
            .Bind(configuration.GetSection("AzureBlobContainersNames"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}