using Microsoft.AspNetCore.Builder;
using Order.Endpoints.Endpoints;

namespace Order.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapOrderApi(this WebApplication app)
    {
        app.MapOrderEndpoints();
        app.MapDeliveryMethodEndpoints();
        
        return app;
    }
}