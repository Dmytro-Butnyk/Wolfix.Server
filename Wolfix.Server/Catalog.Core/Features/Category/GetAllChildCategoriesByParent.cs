using System.Net;
using Catalog.Application.Dto.Category.Responses;
using Catalog.Application.Services;
using Catalog.Domain.Projections.Category;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Caching;
using Shared.Core.Endpoints;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Category;

public static class GetAllChildCategoriesByParent
{
    public sealed record Response(Guid Id, string Name, string PhotoUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("child/{parentId:guid}", Handle)
                .WithSummary("Get all child categories by parent");
        }

        private async Task<Results<Ok<IReadOnlyCollection<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid parentId,
            CancellationToken cancellationToken,
            [FromServices] Handler handler,
            [FromQuery] bool withCaching = true)
        {
            Result<IReadOnlyCollection<Response>> getChildCategoriesResult =
                await handler.HandleAsync(parentId, cancellationToken, withCaching);

            if (getChildCategoriesResult.IsFailure)
            {
                return TypedResults.NotFound(getChildCategoriesResult.ErrorMessage);
            }
        
            return TypedResults.Ok(getChildCategoriesResult.Value);
        }
    }

    public sealed class Handler(
        CatalogContext db,
        IAppCache appCache)
    {
        public async Task<Result<IReadOnlyCollection<Response>>> HandleAsync(
            Guid parentId,
            CancellationToken cancellationToken,
            bool withCaching = true)
        {
            if (!await db.Categories.AsNoTracking().AnyAsync(e => e.Id == parentId, cancellationToken))
            {
                return Result<IReadOnlyCollection<Response>>.Failure(
                    $"Parent category with id: {parentId} not found",
                    HttpStatusCode.NotFound
                );
            }

            List<Response> childCategoriesDto;

            if (withCaching)
            {
                var cacheKey = $"child_categories_by_parent_{parentId}";

                childCategoriesDto = await appCache.GetOrCreateAsync(
                    cacheKey,
                    async ctx => await GetFromDb(parentId, ctx),
                    cancellationToken,
                    TimeSpan.FromMinutes(20)
                );
            }
            else
            {
                childCategoriesDto = await GetFromDb(parentId, cancellationToken);
            }

            return Result<IReadOnlyCollection<Response>>.Success(childCategoriesDto);
        }

        private async Task<List<Response>> GetFromDb(Guid parentId, CancellationToken ct)
            => await db.Categories
                .Include(c => c.Parent)
                .AsNoTracking()
                .Where(category => category.Parent != null && category.Parent!.Id == parentId)
                .Select(category => new Response(category.Id, category.Name, category.PhotoUrl))
                .ToListAsync(ct);
    }
}