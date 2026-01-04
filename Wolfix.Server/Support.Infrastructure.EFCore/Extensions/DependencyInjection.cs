using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;
using Support.Domain.Entities;
using Support.Domain.Interfaces;
using Support.Infrastructure.Repositories;

namespace Support.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static async Task EnsureSupportSchemeExistAndMigrateAsync(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<SupportContext>();
        
        await db.Database.MigrateAsync();
    }

    public static IServiceCollection AddSupportDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SupportContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddSupportRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Domain.Entities.Support>, BaseRepository<SupportContext, Domain.Entities.Support>>();
        services.AddScoped<IBaseRepository<SupportRequest>, BaseRepository<SupportContext, SupportRequest>>();

        services.AddScoped<ISupportRepository, SupportRepository>();
        services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
        
        return services;
    }
}