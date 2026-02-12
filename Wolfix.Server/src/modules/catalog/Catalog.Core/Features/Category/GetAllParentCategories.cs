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

public static class GetAllParentCategories
{
    public sealed record Response(Guid Id, string Name, string PhotoUrl);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("parent", Handle)
                .WithSummary("Get all parent categories");
        }

        private static async Task<Ok<IReadOnlyCollection<Response>>> Handle(
            CancellationToken ct,
            [FromServices] Handler handler,
            [FromQuery] bool withCaching = true)
        {
            Result<IReadOnlyCollection<Response>> getParentCategoriesResult = await handler.HandleAsync(ct, withCaching);
        
            return TypedResults.Ok(getParentCategoriesResult.Value);
        }
    }

    public sealed class Handler(
        CatalogContext db,
        IAppCache appCache)
    {
        public async Task<Result<IReadOnlyCollection<Response>>> HandleAsync(
            CancellationToken ct,
            bool withCaching = true)
        {
            const string cacheKey = "all_parent_categories";

            List<Response> parentCategoriesDto;
        
            if (withCaching)
            {
                parentCategoriesDto = await appCache.GetOrCreateAsync(
                    cacheKey, 
                    async ctx => await GetFromDb(ctx),
                    ct, 
                    TimeSpan.FromMinutes(20));
            }
            else
            {
                parentCategoriesDto = await GetFromDb(ct);
            }
        
            return Result<IReadOnlyCollection<Response>>.Success(parentCategoriesDto);
        }

        private async Task<List<Response>> GetFromDb(CancellationToken ct)
            => await db.Categories
                .Include(c => c.Parent)
                .AsNoTracking()
                .Where(category => category.Parent == null)
                .Select(category => new Response(category.Id, category.Name, category.PhotoUrl))
                .ToListAsync(ct);
    }
}