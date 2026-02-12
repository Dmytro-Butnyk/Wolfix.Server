using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Caching;

namespace Shared.Core.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAppCache(this IServiceCollection services)
    {
        services.AddSingleton<IAppCache, AppCache>();

        return services;
    }
}