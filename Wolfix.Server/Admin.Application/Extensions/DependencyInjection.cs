using Admin.Application.EventHandlers;
using Admin.Application.Interfaces;
using Admin.Application.Services;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Interfaces;

namespace Admin.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }

    public static IServiceCollection AddAdminEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<GetAdminProfileId, Guid>, GetAdminProfileIdEventHandler>();
        
        return services;
    }
}