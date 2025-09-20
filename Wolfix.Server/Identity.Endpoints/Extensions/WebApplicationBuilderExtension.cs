using Identity.Application.Extensions;
using Identity.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, string connectionString, string tokenIssuer, string tokenAudience, string tokenKey, string tokenLifetime)
    {
        services.AddIdentityDbContext(connectionString);
        services.AddIdentityCore();
        services.AddIdentityStore();
        services.AddIdentityJwtOptions(
            tokenIssuer,
            tokenAudience,
            tokenKey,
            tokenLifetime
        );
        
        services.AddIdentityApplicationServices();
        services.AddIdentityEventHandlers();
        
        return services;
    }
}