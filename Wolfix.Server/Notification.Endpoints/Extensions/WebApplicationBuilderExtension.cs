using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Extensions;
using Notification.Infrastructure.SignalR;
using Notification.Infrastructure.SignalR.Options;

namespace Notification.Endpoints.Extensions;

public static class WebApplicationBuilderExtension
{
    public static IServiceCollection AddNotificationModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSignalR()
            .AddHubOptions<NotificationHub>(options =>
            {
                // options.MaximumReceiveMessageSize = 64 * 1024;
                options.EnableDetailedErrors = true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });
        services.AddSingleton<NotificationHub>();
        
        services.AddNotificationServices();
        services.AddNotificationEventHandlers();
        
        return services;
    }
}