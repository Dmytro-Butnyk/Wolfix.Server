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
        
        group.MapGet("child/{parentId:guid}", GetAllChildCategoriesByParent)
            .WithSummary("Get all child categories by parent");
    }

    private static void MapManageEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("", AddParent)
            .WithSummary("Add parent category");
        
        group.MapPatch("{categoryId:guid}", ChangeParent)
            .WithSummary("Change parent category");

        group.MapDelete("{categoryId:guid}", DeleteCategory)
            .WithSummary("Delete category");
        
        group.MapPost("{parentId:guid}", AddChild)
            .WithSummary("Add child category");
        
        group.MapPatch("child/{childCategoryId:guid}", ChangeChild)
            .WithSummary("Change child category");
        
        group.MapPost("child/{childCategoryId:guid}/attributes", AddAttribute)
            .WithSummary("Add attribute to child category");
        
        group.MapPatch("child/{childCategoryId:guid}/attributes/{attributeId:guid}", ChangeAttribute)
            .WithSummary("Change attribute of child category");
        
        group.MapDelete("child/{childCategoryId:guid}/attributes/{attributeId:guid}", DeleteAttribute)
            .WithSummary("Delete attribute of child category");

        group.MapPost("child/{childCategoryId:guid}/variants", AddVariant)
            .WithSummary("Add variant to child category");

        group.MapPatch("child/{childCategoryId:guid}/variants/{variantId:guid}", ChangeVariant)
            .WithSummary("Change variant of child category");
        
        group.MapDelete("child/{childCategoryId:guid}/variants/{variantId:guid}", DeleteVariant)
            .WithSummary("Delete variant of child category");
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

    private static async Task<Results<NoContent, NotFound<string>>> DeleteCategory(
        [FromRoute] Guid categoryId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult deleteCategoryResult = await categoryService.DeleteCategoryAsync(categoryId, ct);
        
        if (!deleteCategoryResult.IsSuccess)
        {
            return TypedResults.NotFound(deleteCategoryResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
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

    private static async Task<Results<Ok<ChildCategoryDto>, NotFound<string>, Conflict<string>, BadRequest<string>>> ChangeChild(
        [FromBody] ChangeChildCategoryDto request,
        [FromRoute] Guid childCategoryId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        Result<ChildCategoryDto> changeChildCategoryResult = await categoryService.ChangeChildAsync(request, childCategoryId, ct);

        if (!changeChildCategoryResult.IsSuccess)
        {
            return changeChildCategoryResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeChildCategoryResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(changeChildCategoryResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeChildCategoryResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(changeChildCategoryResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> AddAttribute(
        [FromBody] AddCategoryAttributeDto request,
        [FromRoute] Guid childCategoryId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult addCategoryAttributeResult = await categoryService.AddAttributeAsync(request, childCategoryId, ct);

        if (!addCategoryAttributeResult.IsSuccess)
        {
            return addCategoryAttributeResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addCategoryAttributeResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(addCategoryAttributeResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addCategoryAttributeResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<CategoryAttributeDto>, NotFound<string>, Conflict<string>, BadRequest<string>>> ChangeAttribute(
        [FromBody] ChangeCategoryAttributeDto request,
        [FromRoute] Guid childCategoryId,
        [FromRoute] Guid attributeId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        Result<CategoryAttributeDto> changeCategoryAttributeResult = await categoryService.ChangeAttributeAsync(request, childCategoryId,
            attributeId, ct);

        if (!changeCategoryAttributeResult.IsSuccess)
        {
            return changeCategoryAttributeResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeCategoryAttributeResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(changeCategoryAttributeResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeCategoryAttributeResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(changeCategoryAttributeResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>>> DeleteAttribute(
        [FromRoute] Guid childCategoryId,
        [FromRoute] Guid attributeId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult deleteAttributeResult = await categoryService.DeleteAttributeAsync(childCategoryId, attributeId, ct);
        
        if (!deleteAttributeResult.IsSuccess)
        {
            return TypedResults.NotFound(deleteAttributeResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, Conflict<string>, BadRequest<string>>> AddVariant(
        [FromBody] AddCategoryVariantDto request,
        [FromRoute] Guid childCategoryId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult addCategoryVariantResult = await categoryService.AddVariantAsync(request, childCategoryId, ct);

        if (!addCategoryVariantResult.IsSuccess)
        {
            return addCategoryVariantResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addCategoryVariantResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(addCategoryVariantResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addCategoryVariantResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<CategoryVariantDto>, NotFound<string>, Conflict<string>, BadRequest<string>>> ChangeVariant(
        [FromBody] ChangeCategoryVariantDto request,
        [FromRoute] Guid childCategoryId,
        [FromRoute] Guid variantId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        Result<CategoryVariantDto> changeCategoryVariantResult = await categoryService.ChangeVariantAsync(request, childCategoryId,
            variantId, ct);

        if (!changeCategoryVariantResult.IsSuccess)
        {
            return changeCategoryVariantResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeCategoryVariantResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(changeCategoryVariantResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeCategoryVariantResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(changeCategoryVariantResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>>> DeleteVariant(
        [FromRoute] Guid childCategoryId,
        [FromRoute] Guid variantId,
        [FromServices] ICategoryService categoryService,
        CancellationToken ct)
    {
        VoidResult deleteVariantResult = await categoryService.DeleteVariantAsync(childCategoryId, variantId, ct);

        if (!deleteVariantResult.IsSuccess)
        {
            return TypedResults.NotFound(deleteVariantResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }
}