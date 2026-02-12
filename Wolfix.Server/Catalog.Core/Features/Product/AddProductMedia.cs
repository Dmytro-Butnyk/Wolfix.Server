using System.Net;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class AddProductMedia
{
    public sealed class Request
    {
        public Guid ProductId { get; init; }
        public IFormFile? Media { get; init; }
        public string ContentType { get; init; }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("add-product-media", Handle)
                .DisableAntiforgery()
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Add product media");
        }

        private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> Handle(
            [FromForm] Request request,
            [FromServices] CatalogContext db,
            [FromServices] EventBus eventBus,
            CancellationToken ct)
        {
            if (!Enum.TryParse(request.ContentType, out BlobResourceType blobResourceType))
            {
                return TypedResults.BadRequest("Invalid blob resource type");
            }

            var product = await db.Products
                .Include("_productMedias")
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {request.ProductId} not found");
            }

            if (product.ProductMedias.Count >= 10)
            {
                return TypedResults.BadRequest("Product can not have more than 10 media");
            }

            if (request.Media is null)
            {
                return TypedResults.BadRequest("Media is null");
            }

            VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
                new ProductMediaAdded(
                    request.ProductId,
                    new MediaEventDto(blobResourceType, request.Media, true)),
                ct);

            if (eventResult.IsFailure)
            {
                return eventResult.StatusCode switch
                {
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(eventResult.ErrorMessage),
                    HttpStatusCode.NotFound => TypedResults.NotFound(eventResult.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(AddProductMedia),
                        nameof(Handle),
                        eventResult.StatusCode
                    )
                };
            }

            return TypedResults.NoContent();
        }
    }
}