using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Category;

public static class GetAllAttributesByCategory
{
    public sealed record Response(Guid Id, string Key);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("child/{childId:guid}/attributes", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Get all attributes by specific category");
        }

        private static async Task<Results<Ok<IReadOnlyCollection<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid childId,
            CancellationToken ct,
            [FromServices] Handler handler)
        {
            Result<IReadOnlyCollection<Response>> result = await handler.HandleAsync(childId, ct);

            if (result.IsFailure)
            {
                return TypedResults.NotFound(result.ErrorMessage);
            }

            return TypedResults.Ok(result.Value);
        }
    }

    public sealed class Handler(CatalogContext db)
    {
        public async Task<Result<IReadOnlyCollection<Response>>> HandleAsync(
            Guid childId,
            CancellationToken ct)
        {
            var category = await db.Categories
                .AsNoTracking()
                .Include("_productAttributes")
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == childId, ct);

            if (category is null)
            {
                return Result<IReadOnlyCollection<Response>>.Failure(
                    $"Category with id: {childId} not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!category.IsChild)
            {
                return Result<IReadOnlyCollection<Response>>.Failure($"Category with id: {childId} is not a child category");
            }

            IReadOnlyCollection<Response> response = category.ProductAttributes
                .Select(attribute => new Response(attribute.Id, attribute.Key))
                .ToList();

            return Result<IReadOnlyCollection<Response>>.Success(response);
        }
    }
}
