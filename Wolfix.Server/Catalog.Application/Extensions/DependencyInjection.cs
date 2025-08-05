using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Caching;
using Shared.Application.Interfaces;

namespace Catalog.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}