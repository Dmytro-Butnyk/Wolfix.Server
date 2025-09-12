using System.Net;
using Catalog.Application.Dto.Category;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Application.Dto.Category.Responses;
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
        MapManageEndpoints(categoryGroup);
    }

    private static void MapGetEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("parent", GetAllParentCategories)
            .WithSummary("Get all parent categories");
        
        group.MapPatch("{categoryId:guid}", ChangeParent)
            .WithSummary("Change parent category");
        
        group.MapGet("child/{parentId:guid}", GetAllChildCategoriesByParent)
            .WithSummary("Get all child categories by parent");
        
        //todo: change child category
    }

    private static void MapManageEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("", AddParent)
            .WithSummary("Add parent category");
        
        group.MapPost("{parentId:guid}", AddChild)
            .WithSummary("Add child category");
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

    private static async Task<Results<NoContent, Conflict<string>, BadRequest<string>>> AddParent(
        [FromBody] AddParentCategoryDto request,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult addParentCategoryResult = await categoryService.AddParentAsync(request, ct);

        if (!addParentCategoryResult.IsSuccess)
        {
            return addParentCategoryResult.StatusCode switch
            {
                HttpStatusCode.Conflict => TypedResults.Conflict(addParentCategoryResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addParentCategoryResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<ParentCategoryDto>, NotFound<string>, Conflict<string>, BadRequest<string>>> ChangeParent(
        [FromBody] ChangeParentCategoryDto request,
        [FromRoute] Guid categoryId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        Result<ParentCategoryDto> changeParentCategoryResult = await categoryService.ChangeParentAsync(request, categoryId, ct);

        if (!changeParentCategoryResult.IsSuccess)
        {
            return changeParentCategoryResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeParentCategoryResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(changeParentCategoryResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeParentCategoryResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }

        return TypedResults.Ok(changeParentCategoryResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> AddChild(
        [FromBody] AddChildCategoryDto request,
        [FromRoute] Guid parentId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult addChildCategoryResult = await categoryService.AddChildAsync(request, parentId, ct);

        if (!addChildCategoryResult.IsSuccess)
        {
            return addChildCategoryResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addChildCategoryResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(addChildCategoryResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addChildCategoryResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }
}