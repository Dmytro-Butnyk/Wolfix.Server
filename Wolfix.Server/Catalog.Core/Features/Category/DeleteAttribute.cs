using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Category;

public static class DeleteAttribute
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/categories/child/{childCategoryId:guid}/attributes/{attributeId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithTags("Categories")
                .WithSummary("Delete attribute of child category");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid childCategoryId,
            [FromRoute] Guid attributeId,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var childCategory = await db.Categories
                .Include(c => c.Parent)
                .Include("_productAttributes")
                .FirstOrDefaultAsync(c => c.Id == childCategoryId, ct);

            if (childCategory is null)
            {
                return TypedResults.NotFound($"Category with id: {childCategoryId} not found");
            }

            if (!childCategory.IsChild)
            {
                return TypedResults.BadRequest($"Category with id: {childCategoryId} is not a child category");
            }

            var deleteAttributeResult = childCategory.RemoveProductAttribute(attributeId);

            if (deleteAttributeResult.IsFailure)
            {
                return TypedResults.NotFound(deleteAttributeResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}
