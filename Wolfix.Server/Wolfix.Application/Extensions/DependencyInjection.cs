using Microsoft.Extensions.DependencyInjection;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Services;
using Wolfix.Application.Shared.Caching;
using Wolfix.Application.Shared.Interfaces;

namespace Wolfix.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAppCache(this IServiceCollection services)
    {
        services.AddSingleton<IAppCache, AppCache>();

        return services;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}