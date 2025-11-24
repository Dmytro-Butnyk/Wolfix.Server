using Microsoft.AspNetCore.Builder;
using Notification.Endpoints.Endpoints;

namespace Notification.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapNotificationApi(this WebApplication app)
    {
        app.MapNotificationEndpoints();
        
        return app;
    }
}