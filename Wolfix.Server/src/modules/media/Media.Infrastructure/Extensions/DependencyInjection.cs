using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Media.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Media.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static async Task EnsureMediaSchemeExistAndMigrateAsync(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<MediaContext>();
        
        await db.Database.MigrateAsync();
    }
    
    public static IServiceCollection AddMediaDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MediaContext>(options => 
            options.UseNpgsql(connectionString));
        
        return services;
    }
    
    public static IServiceCollection AddMediaRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<BlobResource>), typeof(BaseRepository<MediaContext, BlobResource>));
        
        services.AddScoped<IBlobResourceRepository, BlobResourceRepository>();

        return services;
    }
    
    public static IServiceCollection AddAzureBlobRepository(this IServiceCollection services)
    {
        services.AddScoped<IAzureBlobRepository, AzureBlobRepository>();
        
        return services;
    }
    
}