using Admin.Domain.Interfaces;
using Admin.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Admin.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static async Task EnsureAdminSchemeExistAndMigrateAsync(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<AdminContext>();
        
        await db.Database.MigrateAsync();
    }
    
    public static IServiceCollection AddAdminDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AdminContext>(options => 
            options.UseNpgsql(connectionString));
        
        return services;
    }

    public static IServiceCollection AddAdminRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Domain.AdminAggregate.Admin>, BaseRepository<AdminContext, Domain.AdminAggregate.Admin>>();

        services.AddScoped<IAdminRepository, AdminRepository>();

        return services;
    }
}