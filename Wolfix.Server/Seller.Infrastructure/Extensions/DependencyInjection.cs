using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Seller.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSellerDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SellerContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddSellerRepositories(this IServiceCollection services)
    {
        //todo: add repositories

        return services;
    }
}