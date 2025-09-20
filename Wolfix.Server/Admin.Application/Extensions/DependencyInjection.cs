using Admin.Application.Interfaces;
using Admin.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}