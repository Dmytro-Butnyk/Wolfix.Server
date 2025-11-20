using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Caching;
using Shared.Application.Interfaces;

namespace Shared.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAppCache(this IServiceCollection services)
    {
        services.AddSingleton<IAppCache, AppCache>();

        return services;
    }
}