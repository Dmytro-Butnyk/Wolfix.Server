using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;

namespace Catalog.Core.Features.Product;

public static class ChangeProductMainPhoto
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("product/{productId:guid}/new-main-photo/{newMainPhotoId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Change product main photo");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid productId,
            [FromRoute] Guid newMainPhotoId,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var product = await db.Products
                .Include("_productMedias")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            var changeMainPhotoResult = product.ChangeMainPhoto(newMainPhotoId);

            if (changeMainPhotoResult.IsFailure)
            {
                return changeMainPhotoResult.StatusCode switch
                {
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(changeMainPhotoResult.ErrorMessage),
                    HttpStatusCode.NotFound => TypedResults.NotFound(changeMainPhotoResult.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(ChangeProductMainPhoto),
                        nameof(Handle),
                        changeMainPhotoResult.StatusCode
                    )
                };
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}