using System.Net;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Application.Dto.Category.Responses;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Category;

public static class ChangeParent
{
    public sealed record Response(string Name, string? Description);
    
    public sealed record Request(string Name, string? Description);
    
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/categories/{categoryId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Change parent category")
                .WithTags("Categories");
        }

        private async Task<Results<Ok<Response>, NotFound<string>, Conflict<string>, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromRoute] Guid categoryId,
            [FromServices] Handler handler,
            CancellationToken ct)
        {
            Result<Response> result = await handler.HandleAsync(request, categoryId, ct);

            if (result.IsFailure)
            {
                return result.StatusCode switch
                {
                    HttpStatusCode.NotFound => TypedResults.NotFound(result.ErrorMessage),
                    HttpStatusCode.Conflict => TypedResults.Conflict(result.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(ChangeParent),
                        nameof(Handle),
                        result.StatusCode
                    )
                };
            }

            return TypedResults.Ok(result.Value);
        }
    }

    public sealed class Handler(CatalogContext db)
    {
        public async Task<Result<Response>> HandleAsync(Request request, Guid categoryId, CancellationToken ct)
        {
            var parentCategory = await db.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);

            if (parentCategory is null)
            {
                return Result<Response>.Failure(
                    $"Category with id: {categoryId} not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!parentCategory.IsParent)
            {
                return Result<Response>.Failure(
                    $"Category with id: {categoryId} is not a parent category",
                    HttpStatusCode.Conflict
                );
            }

            bool isExist = await db.Categories
                .AsNoTracking()
                .AnyAsync(category => EF.Functions.ILike(category.Name, request.Name), ct);

            if (isExist)
            {
                return Result<Response>.Failure(
                    $"Category with name: {request.Name} already exists",
                    HttpStatusCode.Conflict
                );
            }

            bool isNothingChanged = true;

            VoidResult changeCategoryName = parentCategory.ChangeName(request.Name);

            if (changeCategoryName.IsFailure)
            {
                return Result<Response>.Failure(changeCategoryName);
            }

            isNothingChanged = false;

            VoidResult changeCategoryDescription = parentCategory.ChangeDescription(request.Description);

            if (changeCategoryDescription.IsFailure)
            {
                return Result<Response>.Failure(changeCategoryDescription);
            }

            isNothingChanged = false;

            if (isNothingChanged)
            {
                return Result<Response>.Failure("The same data provided");
            }

            await db.SaveChangesAsync(ct);

            Response dto = new(request.Name, request.Description);
            return Result<Response>.Success(dto);
        }
    }
}
