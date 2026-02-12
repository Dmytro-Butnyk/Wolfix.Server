using System.Net;
using Catalog.Application.Services;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Models;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Core.Features.Category;

public static class AddChild
{
    public sealed record Request(IFormFile Photo, string Name, string? Description,
        IReadOnlyCollection<string> AttributeKeys, IReadOnlyCollection<string> VariantKeys);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("{parentId:guid}", Handle)
                .DisableAntiforgery()
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Add child category");
        }

        private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> Handle(
            [FromForm] Request request,
            [FromRoute] Guid parentId,
            [FromServices] Handler handler,
            CancellationToken ct)
        {
            VoidResult addChildCategoryResult = await handler.HandleAsync(request, parentId, ct);

            if (addChildCategoryResult.IsFailure)
            {
                return addChildCategoryResult.StatusCode switch
                {
                    HttpStatusCode.NotFound => TypedResults.NotFound(addChildCategoryResult.ErrorMessage),
                    HttpStatusCode.Conflict => TypedResults.Conflict(addChildCategoryResult.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(addChildCategoryResult.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(Category),
                        nameof(AddChild),
                        addChildCategoryResult.StatusCode
                    )
                };
            }
        
            return TypedResults.NoContent();
        }
    }

    public sealed class Handler(CatalogContext db)
    {
        public async Task<VoidResult> HandleAsync(Request request, Guid parentId, CancellationToken ct)
        {
            CategoryAggregate? parentCategory = await db.Categories
                .Include(category => category.Parent)
                .FirstOrDefaultAsync(category => category.Id == parentId, ct);

            if (parentCategory is null)
            {
                return VoidResult.Failure(
                    $"Parent category with id: {parentId} not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!parentCategory.IsParent)
            {
                return VoidResult.Failure(
                    $"Category with id: {parentId} already has a parent",
                    HttpStatusCode.Conflict
                );
            }

            bool alreadyExistsWithThisName = await db.Categories
                .AsNoTracking()
                .AnyAsync(category => EF.Functions.ILike(category.Name, request.Name), ct);
            
            if (alreadyExistsWithThisName)
            {
                return VoidResult.Failure(
                    $"Category with name: {request.Name} already exists",
                    HttpStatusCode.Conflict
                );
            }
            
            //todo: пофиксить (ажур аккаунт срок истёк)
            
            // var @event = new AddPhotoForNewCategory
            // {
            //     FileData = request.Photo
            // };
            //
            // Result<CreatedMediaDto> createImageResult =
            //     await eventBus.PublishWithSingleResultAsync<AddPhotoForNewCategory, CreatedMediaDto>(@event, ct);
            //
            // if (createImageResult.IsFailure)
            // {
            //     return VoidResult.Failure(createImageResult);
            // }
            //
            // CreatedMediaDto createdBlobResource = createImageResult.Value!;
            //
            // Result<Category> createCategoryResult = Category.Create(createdBlobResource.BlobResourceId, createdBlobResource.Url,
            //     request.Name, request.Description, parentCategory);
            
            Result<CategoryAggregate> createCategoryResult = CategoryAggregate.Create(Guid.CreateVersion7(), "URL",
                request.Name, request.Description, parentCategory);

            if (createCategoryResult.IsFailure)
            {
                return VoidResult.Failure(createCategoryResult);
            }
            
            CategoryAggregate category = createCategoryResult.Value!;

            VoidResult addAttributesResult = category.AddProductAttributes(request.AttributeKeys);

            if (addAttributesResult.IsFailure)
            {
                return VoidResult.Failure(addAttributesResult);
            }
            
            VoidResult addVariantsResult = category.AddProductVariants(request.VariantKeys);

            if (addVariantsResult.IsFailure)
            {
                return VoidResult.Failure(addVariantsResult);
            }
            
            await db.Categories.AddAsync(category, ct);
            await db.SaveChangesAsync(ct);
            
            return VoidResult.Success();
        }
    }
}