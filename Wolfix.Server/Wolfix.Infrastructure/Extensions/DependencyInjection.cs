using Microsoft.Extensions.DependencyInjection;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Shared.Interfaces;
using Wolfix.Infrastructure.Catalog.Repositories;
using Wolfix.Infrastructure.Shared.Repositories;

namespace Wolfix.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IBlobRepository, BlobRepository>();

        return services;
    }
}