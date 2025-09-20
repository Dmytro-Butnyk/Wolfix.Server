using Admin.Endpoints.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Admin.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapAdminApi(this WebApplication app)
    {
        app.MapAdminEndpoints();

        return app;
    }
}