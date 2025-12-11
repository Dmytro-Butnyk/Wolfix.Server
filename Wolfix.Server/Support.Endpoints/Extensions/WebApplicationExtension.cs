using Microsoft.AspNetCore.Builder;
using Support.Endpoints.Endpoints;

namespace Support.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapSupportApi(this WebApplication app)
    {
        app.MapSupportRequestEndpoints();
        
        return app;
    }
}