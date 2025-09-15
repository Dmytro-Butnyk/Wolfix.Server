using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seller.Domain.Interfaces;
using Seller.Infrastructure.Repositories;

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
        services.AddScoped<ISellerRepository, SellerRepository>();

        return services;
    }
}