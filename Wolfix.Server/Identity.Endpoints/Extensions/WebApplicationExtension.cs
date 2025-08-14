using Identity.Endpoints.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Identity.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapIdentityApi(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        
        return app;
    }
}