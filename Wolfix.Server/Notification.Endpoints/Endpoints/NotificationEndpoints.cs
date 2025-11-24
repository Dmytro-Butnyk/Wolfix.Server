using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Notification.Infrastructure.SignalR;

namespace Notification.Endpoints.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var hubGroup = app.MapGroup("notification")
            .WithTags("Notification");

        hubGroup.MapHub<NotificationHub>("/hub");
    }
}