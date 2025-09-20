using Admin.Endpoints.Extensions;
using Admin.Infrastructure.Extensions;
using Catalog.Endpoints.Extensions;
using Catalog.Infrastructure.Extensions;
using Customer.Endpoints.Extensions;
using Customer.Infrastructure.Extensions;
using Identity.Endpoints.Extensions;
using Identity.Infrastructure;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Helpers;
using Identity.Infrastructure.Identity;
using Media.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Order.Endpoints.Extensions;
using Order.Infrastructure.Extensions;
using Seller.Endpoints.Extensions;
using Seller.Infrastructure.Extensions;

namespace Wolfix.API.Extensions;

public static class WebApplicationExtension
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCatalogApi();
        app.MapIdentityApi();
        app.MapCustomerApi();
        app.MapSellerApi();
        app.MapOrderApi();
        app.MapAdminApi();
    }

    public static async Task EnsureDatabaseExistAndMigrationsApplied(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        
        await services.EnsureCatalogSchemeExistAndMigrateAsync();
        await services.EnsureCustomerSchemeExistAndMigrateAsync();
        await services.EnsureIdentitySchemeExistAndMigrateAsync();
        await services.EnsureMediaSchemeExistAndMigrateAsync();
        await services.EnsureSellerSchemeExistAndMigrateAsync();
        await services.EnsureOrderSchemeExistAndMigrateAsync();
        await services.EnsureAdminSchemeExistAndMigrateAsync();
    }

    public static async Task EnsureRolesValid(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<IdentityContext>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        await context.EnsureAllRolesExist(roleManager);
    }
}