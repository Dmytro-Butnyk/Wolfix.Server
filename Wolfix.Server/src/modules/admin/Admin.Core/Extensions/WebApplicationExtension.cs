namespace Admin.Core.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapAdminApi(this WebApplication app)
    {
        app.MapAdminEndpoints();

        return app;
    }
}