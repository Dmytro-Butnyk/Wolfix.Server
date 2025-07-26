using Microsoft.AspNetCore.Mvc;
using Wolfix.Application.Catalog.Dto.Category;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Domain.Shared;

namespace Wolfix.API.Controllers.Catalog;

[Route("api/сategories")]
[ApiController]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllParentCategories(CancellationToken ct)
    {
        Result<IEnumerable<CategoryShortDto>> getParentCategoriesResult = await categoryService.GetAllParentCategoriesAsync(ct);

        return getParentCategoriesResult.Map<IActionResult>(
            onSuccess: parentCategories => Ok(parentCategories),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }

    [HttpGet("{parentId:guid}")]
    public async Task<IActionResult> GetAllChildCategoriesByParent([FromRoute] Guid parentId, CancellationToken ct)
    {
        Result<IEnumerable<CategoryShortDto>> getChildCategoriesResult = await categoryService.GetAllChildCategoriesByParentAsync(parentId, ct);

        return getChildCategoriesResult.Map<IActionResult>(
            onSuccess: childCategories => Ok(childCategories),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }
}