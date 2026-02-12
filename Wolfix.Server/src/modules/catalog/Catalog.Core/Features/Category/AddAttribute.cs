using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Core.Features.Category;

public static class AddAttribute
{
    public sealed record Request(string Key);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("child/{childCategoryId:guid}/attributes", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Add attribute to child category");
        }

        private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromRoute] Guid childCategoryId,
            [FromServices] CatalogContext db,
            CancellationToken cancellationToken)
        {
            CategoryAggregate? childCategory = await db.Categories
                .Include("Parent")
                .Include("_productAttributes")
                .FirstOrDefaultAsync(c => c.Id == childCategoryId, cancellationToken);

            if (childCategory is null)
            {
                return TypedResults.NotFound($"Child category with id: {childCategoryId} not found");
            }

            if (!childCategory.IsChild)
            {
                return TypedResults.BadRequest($"Category with id: {childCategoryId} is not a child category");
            }

            VoidResult addAttributeResult = childCategory.AddProductAttribute(request.Key);

            if (addAttributeResult.IsFailure)
            {
                return addAttributeResult.StatusCode switch
                {
                    HttpStatusCode.Conflict => TypedResults.Conflict(addAttributeResult.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(addAttributeResult.ErrorMessage)
                };
            }

            await db.SaveChangesAsync(cancellationToken);

            //todo: кидать уведомление продавцу о том что нужно добавить значение для аттрибута

            return TypedResults.NoContent();
        }
    }
}
