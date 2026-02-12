using System.Net;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Category;

public static class AddParent
{
    public sealed record Request(IFormFile Photo, string Name, string? Description);
    
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/categories", Handle)
                .DisableAntiforgery()
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Add parent category")
                .WithTags("Categories");
        }

        private async Task<Results<NoContent, Conflict<string>, BadRequest<string>>> Handle(
            [FromForm] Request request,
            [FromServices] Handler handler,
            CancellationToken ct)
        {
            VoidResult result = await handler.HandleAsync(request, ct);

            if (!result.IsSuccess)
            {
                return result.StatusCode switch
                {
                    HttpStatusCode.Conflict => TypedResults.Conflict(result.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(AddParent),
                        nameof(Handle),
                        result.StatusCode
                    )
                };
            }

            return TypedResults.NoContent();
        }
    }

    public sealed class Handler(CatalogContext db, EventBus eventBus)
    {
        public async Task<VoidResult> HandleAsync(Request request, CancellationToken ct)
        {
            bool isExist = await db.Categories
                .AsNoTracking()
                .AnyAsync(category => EF.Functions.ILike(category.Name, request.Name), ct);

            if (isExist)
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
            // Result<Category> createCategoryResult = Domain.CategoryAggregate.Category.Create(createdBlobResource.BlobResourceId, createdBlobResource.Url,
            //     request.Name, request.Description);
            
            Result<Domain.CategoryAggregate.Category> createCategoryResult = Domain.CategoryAggregate.Category.Create(Guid.CreateVersion7(), "URL",
                request.Name, request.Description);
            
            if (createCategoryResult.IsFailure)
            {
                return VoidResult.Failure(createCategoryResult);
            }
            
            Domain.CategoryAggregate.Category category = createCategoryResult.Value!;

            await db.Categories.AddAsync(category, ct);
            await db.SaveChangesAsync(ct);
            
            return VoidResult.Success();
        }
    }
}
