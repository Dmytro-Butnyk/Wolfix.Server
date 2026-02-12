using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class ChangeProductGeneralInfo
{
    public sealed record Request(string Title, string Description, Guid CategoryId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("product/{productId:guid}/general-info", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Change product general info");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid productId,
            [FromBody] Request request,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var product = await db.Products.FindAsync(new object[] { productId }, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"product with id: {productId} not found");
            }

            var changeTitleResult = product.ChangeTitle(request.Title);
            if (changeTitleResult.IsFailure)
            {
                return TypedResults.BadRequest(changeTitleResult.ErrorMessage);
            }

            var changeDescriptionResult = product.ChangeDescription(request.Description);
            if (changeDescriptionResult.IsFailure)
            {
                return TypedResults.BadRequest(changeDescriptionResult.ErrorMessage);
            }

            var changeCategoryResult = product.ChangeCategory(request.CategoryId);
            if (changeCategoryResult.IsFailure)
            {
                return TypedResults.BadRequest(changeCategoryResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}