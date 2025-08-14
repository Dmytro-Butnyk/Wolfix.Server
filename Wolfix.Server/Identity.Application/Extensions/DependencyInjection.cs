using Identity.Application.Interfaces.Services;
using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}