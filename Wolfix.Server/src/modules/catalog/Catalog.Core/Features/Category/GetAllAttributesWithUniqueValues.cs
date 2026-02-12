using System.Net;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
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

public static class GetAllAttributesWithUniqueValues
{
    public sealed record Response(Guid AttributeId, string Key, IReadOnlyCollection<string> Values);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("child/{childId:guid}/attributes-with-values", Handle)
                .WithSummary("Get all attributes with unique values for filter component");
        }

        private async Task<Results<Ok<IReadOnlyCollection<Response>>, BadRequest<string>, NotFound<string>>> Handle(
            [FromRoute] Guid childId,
            CancellationToken ct,
            [FromServices] Handler handler)
        {
            Result<IReadOnlyCollection<Response>> result = await handler.HandleAsync(childId, ct);
            
            if (result.IsFailure)
            {
                return result.StatusCode switch
                {
                    HttpStatusCode.NotFound => TypedResults.NotFound(result.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(GetAllAttributesWithUniqueValues),
                        nameof(Handle),
                        result.StatusCode
                    )
                };
            }
            
            return TypedResults.Ok(result.Value);
        }
    }

    public sealed class Handler(
        CatalogContext db,
        CategoryDomainService categoryDomainService)
    {
        public async Task<Result<IReadOnlyCollection<Response>>> HandleAsync(
            Guid childCategoryId,
            CancellationToken ct)
        {
            Domain.CategoryAggregate.Category? childCategory =
                await db.Categories
                    .AsNoTracking()
                    .Include("_productAttributes")
                    .Include(c => c.Parent)
                    .FirstOrDefaultAsync(c => c.Id == childCategoryId, ct);
            
            if (childCategory is null)
            {
                return Result<IReadOnlyCollection<Response>>
                    .Failure($"Category with id:{childCategoryId} not found", HttpStatusCode.NotFound);
            }
            
            if (!childCategory.IsChild)
            {
                return Result<IReadOnlyCollection<Response>>
                    .Failure($"Category with id: {childCategoryId} is not a child category");
            }
            
            IReadOnlyCollection<Guid> attributeIds = childCategory.ProductAttributes
                .Select(attribute => attribute.Id)
                .ToList();

            IReadOnlyCollection<AttributeAndUniqueValuesValueObject> attributesAndUniqueValues =
                await categoryDomainService
                    .GetAttributesAndUniqueValuesAsync(childCategoryId, attributeIds, ct);

            IReadOnlyCollection<Response> response =
                attributesAndUniqueValues.Select(anu => 
                    new Response(anu.AttributeId, anu.Key, anu.Values))
                    .ToList();
            
            return Result<IReadOnlyCollection<Response>>.Success(response);   
        }
    }
}