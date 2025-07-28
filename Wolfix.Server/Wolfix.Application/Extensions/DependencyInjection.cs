using Microsoft.Extensions.DependencyInjection;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Services;

namespace Wolfix.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}