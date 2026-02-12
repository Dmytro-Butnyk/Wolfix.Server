using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Category;

public static class AddVariant
{
    public sealed record Request(string Key);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("child/{childCategoryId:guid}/variants", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithTags("Categories")
                .WithSummary("Add variant to child category");
        }

        private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromRoute] Guid childCategoryId,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var childCategory = await db.Categories
                .Include(c => c.Parent)
                .Include("_productVariants")
                .FirstOrDefaultAsync(c => c.Id == childCategoryId, ct);

            if (childCategory is null)
            {
                return TypedResults.NotFound($"Child category with id: {childCategoryId} not found");
            }

            if (!childCategory.IsChild)
            {
                return TypedResults.BadRequest($"Category with id: {childCategoryId} is not a child category");
            }

            var addVariantResult = childCategory.AddProductVariant(request.Key);

            if (addVariantResult.IsFailure)
            {
                return addVariantResult.StatusCode switch
                {
                    HttpStatusCode.Conflict => TypedResults.Conflict(addVariantResult.ErrorMessage),
                    _ => TypedResults.BadRequest(addVariantResult.ErrorMessage)
                };
            }

            await db.SaveChangesAsync(ct);

            //todo: кидать уведомление продавцу о том что нужно добавить значение для варианта

            return TypedResults.NoContent();
        }
    }
}
