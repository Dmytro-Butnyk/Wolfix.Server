using System.Net;
using Catalog.Domain.Services;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Category;

public static class DeleteCategory
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/categories/{categoryId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Admin)
                .WithSummary("Delete category")
                .WithTags("Categories");
        }

        private async Task<Results<NoContent, NotFound<string>>> Handle(
            [FromRoute] Guid categoryId,
            [FromServices] Handler handler,
            CancellationToken ct)
        {
            VoidResult result = await handler.HandleAsync(categoryId, ct);

            if (!result.IsSuccess)
            {
                return TypedResults.NotFound(result.ErrorMessage);
            }

            return TypedResults.NoContent();
        }
    }

    public sealed class Handler(
        CatalogContext db,
        ProductDomainService productDomainService,
        EventBus eventBus)
    {
        public async Task<VoidResult> HandleAsync(Guid categoryId, CancellationToken ct)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, ct);

            if (category is null)
            {
                return VoidResult.Failure(
                    $"Category with id: {categoryId} not found",
                    HttpStatusCode.NotFound
                );
            }

            IReadOnlyCollection<Guid> allMediaIdsOfCategoryProducts =
                await productDomainService.GetAllMediaIdsByCategoryProducts(categoryId, ct);

            if (allMediaIdsOfCategoryProducts.Count > 0)
            {
                VoidResult publishResult = await eventBus.PublishWithoutResultAsync(new CategoryAndProductsDeleted
                {
                    MediaIds = allMediaIdsOfCategoryProducts
                }, ct);

                if (publishResult.IsFailure)
                {
                    return publishResult;
                }
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync(ct);

            return VoidResult.Success();
        }
    }
}
