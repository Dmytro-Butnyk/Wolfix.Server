using Identity.Application.Interfaces.Repositories;
using Identity.Application.Options;
using Identity.Infrastructure.Identity;
using Identity.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddIdentityCore(this IServiceCollection services)
    {
        services.AddIdentityCore<Account>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<IdentityContext>();
        
        return services;
    }

    public static IServiceCollection AddIdentityStore(this IServiceCollection services)
    {
        services.AddScoped<IAuthStore, AuthStore>();

        return services;
    }

    public static IServiceCollection AddIdentityJwtOptions(this IServiceCollection services, string issuer, string audience, string key, string lifetime)
    {
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = issuer;
            options.Audience = audience;
            options.Key = key;
            options.Lifetime = int.Parse(lifetime);
        });

        return services;
    }
}