using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Order.Application.Dto.DeliveryMethod;
using Order.Application.Services;
using Shared.Domain.Models;
using Shared.Endpoints;

namespace Order.Endpoints.Endpoints;

internal static class DeliveryMethodEndpoints
{
    private const string Route = "api/delivery-methods";
    
    public static void MapDeliveryMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var deliveryMethodsGroup = app.MapGroup(Route)
            .WithTags("DeliveryMethods");
        
        deliveryMethodsGroup.MapGet("delivery-methods", GetDeliveryMethods)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Get delivery methods");
    }
    
    private static async Task<Results<Ok<IReadOnlyCollection<DeliveryMethodDto>>, NotFound<string>>> GetDeliveryMethods(
        [FromServices] DeliveryMethodService deliveryMethodService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<DeliveryMethodDto>> getDeliveryMethodsResult = await deliveryMethodService.GetDeliveryMethodsAsync(ct);

        if (getDeliveryMethodsResult.IsFailure)
        {
            return TypedResults.NotFound(getDeliveryMethodsResult.ErrorMessage);
        }

        return TypedResults.Ok(getDeliveryMethodsResult.Value);
    }
}