using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Core.Features.Category;

public static class ChangeChild
{
    public sealed record Request(string Name, string? Description);

    public sealed record Response(string Name, string? Description);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("child/{childCategoryId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Change child category");
        }

        private static async Task<Results<Ok<Response>, NotFound<string>, Conflict<string>, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromRoute] Guid childCategoryId,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            CategoryAggregate? childCategory = await db.Categories
                .Include(category => category.Parent)
                .FirstOrDefaultAsync(category => category.Id == childCategoryId, ct);
        
            if (childCategory is null)
            {
                return TypedResults.NotFound($"Child category with id: {childCategoryId} not found");
            }
        
            if (!childCategory.IsChild)
            {
                return TypedResults.Conflict($"Category with id: {childCategoryId} is not a child category");
            }
            
            bool isNameChanged = request.Name != childCategory.Name;
            bool isDescriptionChanged = request.Description != childCategory.Description;

            if (!isNameChanged && !isDescriptionChanged)
            {
                return TypedResults.BadRequest("The same data provided");
            }

            if (isNameChanged)
            {
                bool isNameExistsExceptCurrentCategory = await db.Categories
                    .AsNoTracking()
                    .AnyAsync(category => category.Id != childCategoryId
                        && EF.Functions.ILike(category.Name, request.Name), ct);
                
                if (isNameExistsExceptCurrentCategory)
                {
                    return TypedResults.Conflict($"Child category with name: {request.Name} already exists");
                }
                
                VoidResult changeCategoryName = childCategory.ChangeName(request.Name);

                if (changeCategoryName.IsFailure)
                {
                    return TypedResults.BadRequest(changeCategoryName.ErrorMessage);
                }
            }

            if (isDescriptionChanged)
            {
                VoidResult changeCategoryDescription = childCategory.ChangeDescription(request.Description);

                if (changeCategoryDescription.IsFailure)
                {
                    return TypedResults.BadRequest(changeCategoryDescription.ErrorMessage);
                }
            }
            
            await db.SaveChangesAsync(ct);

            Response dto = new(request.Name, request.Description);
            return TypedResults.Ok(dto);
        }
    }
}