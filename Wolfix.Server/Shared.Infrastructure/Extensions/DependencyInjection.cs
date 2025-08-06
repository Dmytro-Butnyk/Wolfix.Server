using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Shared.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBlobRepository, BlobRepository>();
        
        return services;
    }
}