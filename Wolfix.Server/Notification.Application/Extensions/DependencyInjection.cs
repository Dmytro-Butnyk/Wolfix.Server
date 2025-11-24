using Customer.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.EventHandlers;
using Notification.Infrastructure.Interfaces;
using Notification.Infrastructure.SignalR.Options;
using Shared.IntegrationEvents.Interfaces;

namespace Notification.Application.Extensions;

public static class DependencyInjection
{
    
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMapping, ConnectionMapping>();
        return services;
    }

    public static IServiceCollection AddNotificationEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<FetchedUserFavorites>, FetchedUserFavoritesEventHandler>();

        return services;
    }
}