using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CatalogContext>(options => 
            options.UseNpgsql(connectionString));
        
        return services;
    }
    
    public static IServiceCollection AddCatalogRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<Product>), typeof(BaseRepository<CatalogContext, Product>));
        services.AddScoped(typeof(IBaseRepository<Category>), typeof(BaseRepository<CatalogContext, Category>));
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}