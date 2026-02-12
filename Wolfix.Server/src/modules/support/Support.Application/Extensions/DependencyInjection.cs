using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Interfaces;
using Support.Application.EventHandlers;
using Support.Application.Services;

namespace Support.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<SupportRequestService>();
        services.AddScoped<SupportService>();

        return services;
    }

    public static IServiceCollection AddSupportEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<GetSupportProfileId, Guid>, GetSupportProfileIdEventHandler>();
        
        return services;
    }
}