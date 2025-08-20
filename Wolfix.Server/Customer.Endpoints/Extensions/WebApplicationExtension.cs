using Customer.Endpoints.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Customer.Endpoints.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication MapCustomerApi(this WebApplication app)
    {
        app.MapCustomerEndpoints();
        
        return app;
    }
}