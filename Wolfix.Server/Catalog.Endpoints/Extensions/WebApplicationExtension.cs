using Catalog.Endpoints.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Catalog.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapCatalogApi(this WebApplication app)
    {
        app.MapProductEndpoints();
        app.MapCategoryEndpoints();
        
        return app;
    }
}