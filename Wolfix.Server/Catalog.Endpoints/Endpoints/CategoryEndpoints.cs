using Catalog.Application.Dto.Category;
using Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;

namespace Catalog.Endpoints.Endpoints;

internal static class CategoryEndpoints
{
    private const string Route = "api/categories";
    
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categoryGroup = app.MapGroup(Route)
            .WithTags("Categories");
        
        MapGetEndpoints(categoryGroup);
    }

    private static void MapGetEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("parent", GetAllParentCategories);
        group.MapGet("child/{parentId:guid}", GetAllChildCategoriesByParent);
    }

    private static async Task<Ok<IReadOnlyCollection<CategoryShortDto>>> GetAllParentCategories(
        CancellationToken ct,
        [FromServices] ICategoryService categoryService)
    {
        Result<IReadOnlyCollection<CategoryShortDto>> getParentCategoriesResult = await categoryService.GetAllParentCategoriesAsync(ct);
        
        return TypedResults.Ok(getParentCategoriesResult.Value);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<CategoryShortDto>>, NotFound<string>>> GetAllChildCategoriesByParent(
        [FromRoute] Guid parentId,
        CancellationToken ct,
        [FromServices] ICategoryService categoryService)
    {
        Result<IReadOnlyCollection<CategoryShortDto>> getChildCategoriesResult =
            await categoryService.GetAllChildCategoriesByParentAsync(parentId, ct);

        if (!getChildCategoriesResult.IsSuccess)
        {
            return TypedResults.NotFound(getChildCategoriesResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getChildCategoriesResult.Value);
    }
}