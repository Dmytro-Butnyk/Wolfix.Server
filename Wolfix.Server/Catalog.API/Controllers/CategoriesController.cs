using Catalog.Application.Dto.Category;
using Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Models;

namespace Catalog.API.Controllers;

[Route("api/categories")]
[ApiController]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet("parent")]
    public async Task<IActionResult> GetAllParentCategories(CancellationToken ct)
    {
        Result<IReadOnlyCollection<CategoryShortDto>> getParentCategoriesResult = await categoryService.GetAllParentCategoriesAsync(ct);

        return getParentCategoriesResult.Map<IActionResult>(
            onSuccess: parentCategories => Ok(parentCategories),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }

    [HttpGet("child/{parentId:guid}")]
    public async Task<IActionResult> GetAllChildCategoriesByParent([FromRoute] Guid parentId, CancellationToken ct)
    {
        Result<IReadOnlyCollection<CategoryShortDto>> getChildCategoriesResult =
            await categoryService.GetAllChildCategoriesByParentAsync(parentId, ct);

        return getChildCategoriesResult.Map<IActionResult>(
            onSuccess: childCategories => Ok(childCategories),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }
}