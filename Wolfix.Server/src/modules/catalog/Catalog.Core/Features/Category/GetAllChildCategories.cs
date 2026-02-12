using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Category;

public static class GetAllChildCategories
{
    public sealed record Response(Guid Id, string Name);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("child", Handle)
                .WithSummary("Get all child categories");
        }

        private static async Task<Ok<List<Response>>> Handle(
            CancellationToken ct,
            [FromServices] CatalogContext db)
            => TypedResults.Ok(await db.Categories
                .Include(c => c.Parent)
                .AsNoTracking()
                .Where(category => category.Parent != null)
                .Select(category => new Response(category.Id, category.Name))
                .ToListAsync(ct));
    }
}