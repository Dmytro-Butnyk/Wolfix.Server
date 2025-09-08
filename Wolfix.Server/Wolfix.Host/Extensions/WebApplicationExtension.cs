using Catalog.Endpoints.Extensions;
using Customer.Endpoints.Extensions;
using Identity.Endpoints.Extensions;
using Identity.Infrastructure;
using Identity.Infrastructure.Helpers;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Wolfix.Host.Extensions;

public static class WebApplicationExtension
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCatalogApi();
        app.MapIdentityApi();
        app.MapCustomerApi();
    }

    public static async Task EnsureAllRolesExist(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<IdentityContext>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        await context.EnsureAllRolesExist(roleManager);
    }
}