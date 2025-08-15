using Catalog.Endpoints.Extensions;
using Identity.Endpoints.Extensions;

namespace Wolfix.Host.Extensions;

public static class WebApplicationExtension
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCatalogApi();
        app.MapIdentityApi();
    }
}