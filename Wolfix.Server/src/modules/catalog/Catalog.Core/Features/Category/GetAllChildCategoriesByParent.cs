using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Caching;
using Shared.Core.Endpoints;

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

        private static async Task<Results<Ok<List<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid parentId,
            [FromServices] CatalogContext db,
            [FromServices] IAppCache appCache,
            CancellationToken cancellationToken,
            [FromQuery] bool withCaching = true)
        {
            if (!await db.Categories.AsNoTracking().AnyAsync(e => e.Id == parentId, cancellationToken))
            {
                return TypedResults.NotFound($"Parent category with id: {parentId} not found");
            }

            List<Response> childCategoriesDto;

            if (withCaching)
            {
                var cacheKey = $"child_categories_by_parent_{parentId}";

                childCategoriesDto = await appCache.GetOrCreateAsync(
                    cacheKey,
                    async ctx => await GetFromDb(ctx),
                    cancellationToken,
                    TimeSpan.FromMinutes(20)
                );
            }
            else
            {
                childCategoriesDto = await GetFromDb(cancellationToken);
            }

            return TypedResults.Ok(childCategoriesDto);

            async Task<List<Response>> GetFromDb(CancellationToken ct) =>
                await db.Categories.Include(c => c.Parent)
                    .AsNoTracking()
                    .Where(category => category.Parent != null && category.Parent!.Id == parentId)
                    .Select(category => new Response(category.Id, category.Name, category.PhotoUrl))
                    .ToListAsync(ct);
        }
    }
}