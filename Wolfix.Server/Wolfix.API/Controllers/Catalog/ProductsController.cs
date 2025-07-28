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
        Result<IEnumerable<ProductShortDto>> getProductsByCategoryResult =
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
        Result<IEnumerable<ProductShortDto>> getProductsWithDiscountResult =
            await productService.GetForPageWithDiscountAsync(page, pageSize, ct);

        return getProductsWithDiscountResult.Map<IActionResult>(
            onSuccess: productsWithDiscount => Ok(productsWithDiscount),
            onFailure: errorMessage => NotFound(errorMessage)
        );
    }
    
    // todo: get random products method.
    
}