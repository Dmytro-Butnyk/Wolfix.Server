using Microsoft.AspNetCore.Mvc;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Domain.Shared;

namespace Wolfix.API.Controllers.Catalog;

[Route("api/products")]
[ApiController]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet("{childCategoryId:guid}")]
    public async Task<IActionResult> GetAllProductsByCategory([FromRoute] Guid childCategoryId, CancellationToken ct)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getProductsByCategoryResult =
            await productService.GetAllByCategoryIdAsync(childCategoryId, ct);

        return getProductsByCategoryResult.Map<IActionResult>(
            onSuccess: parentCategories => Ok(parentCategories),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }

    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetProductsWithDiscountForPage([FromRoute] int page, [FromQuery] int pageSize,
        CancellationToken ct)
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than 0");
        }

        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0");
        }
        
        Result<IReadOnlyCollection<ProductShortDto>> getProductsWithDiscountResult =
            await productService.GetForPageWithDiscountAsync(page, pageSize, ct);

        return getProductsWithDiscountResult.Map<IActionResult>(
            onSuccess: productsWithDiscount => Ok(productsWithDiscount),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }

    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommendedProductsForPage([FromQuery] int pageSize, [FromQuery] List<Guid> visitedCategoriesIds,
        CancellationToken ct)
    {
        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0");
        }

        if (visitedCategoriesIds.Count == 0)
        {
            return BadRequest("Visited categories must be not empty");
        }
        
        Result<IReadOnlyCollection<ProductShortDto>> getRecommendedProductsResult =
            await productService.GetRecommendedForPageAsync(pageSize, visitedCategoriesIds, ct);

        return getRecommendedProductsResult.Map<IActionResult>(
            onSuccess: recommendedProducts => Ok(recommendedProducts),    
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }
}