using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Seller.Endpoints.Endpoints;

internal static class SellerApplicationEndpoints
{
    private const string Route = "api/seller-applications";

    public static void MapSellerApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var sellerApplicationGroup = app.MapGroup(Route)
            .WithTags("Seller Application");
    }
    
    
}