using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seller.Domain.Interfaces;
using Seller.Domain.SellerApplicationAggregate;
using Seller.Infrastructure.Repositories;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static async Task EnsureSellerSchemeExistAndMigrateAsync(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<SellerContext>();
        
        await db.Database.MigrateAsync();
    }
    
    public static IServiceCollection AddSellerDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SellerContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddSellerRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Domain.SellerAggregate.Seller>, BaseRepository<SellerContext, Domain.SellerAggregate.Seller>>();
        services.AddScoped<IBaseRepository<SellerApplication>, BaseRepository<SellerContext, SellerApplication>>();
        
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ISellerApplicationRepository, SellerApplicationRepository>();

        return services;
    }
}