using Microsoft.AspNetCore.Mvc;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Shared.Dto;
using Wolfix.Domain.Shared;

namespace Wolfix.API.Controllers.Catalog;

[Route("api/products")]
[ApiController]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet("category/{childCategoryId:guid}/page/{page:int}")]
    public async Task<IActionResult> GetAllProductsByCategoryForPage([FromRoute] Guid childCategoryId,
        [FromQuery] int pageSize, [FromQuery] Guid? cursor, CancellationToken ct)
    {
        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0");
        }
        
        Result<CursorPaginationDto<ProductShortDto>> getProductsByCategoryResult =
            await productService.GetForPageByCategoryIdAsync(pageSize, childCategoryId, cursor, ct);

        return getProductsByCategoryResult.Map<IActionResult>(
            onSuccess: productsByCategory => Ok(productsByCategory),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }

    [HttpGet("with-discount/page/{page:int}")]
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
        
        Result<PaginationDto<ProductShortDto>> getProductsWithDiscountResult =
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