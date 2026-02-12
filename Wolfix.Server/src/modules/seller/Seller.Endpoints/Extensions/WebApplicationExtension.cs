using Microsoft.AspNetCore.Builder;
using Seller.Endpoints.Endpoints;

namespace Seller.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapSellerApi(this WebApplication app)
    {
        app.MapSellerEndpoints();
        app.MapSellerApplicationEndpoints();
        
        return app;
    }
}