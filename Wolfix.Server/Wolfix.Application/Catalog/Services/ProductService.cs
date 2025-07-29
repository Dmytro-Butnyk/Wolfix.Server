using System.Net;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Product;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetAllByCategoryIdAsync(Guid childCategoryId,
        CancellationToken ct)
    {
        IReadOnlyCollection<ProductShortProjection> productsByCategory =
            await productRepository.GetAllByCategoryIdAsNoTrackingAsync(childCategoryId, ct);

        if (productsByCategory.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                $"Products by category: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }

        List<ProductShortDto> productShortDtos = productsByCategory
            .Select(product => product.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        IReadOnlyCollection<ProductShortProjection> productsWithDiscount =
            await productRepository.GetForPageWithDiscountAsync(page, pageSize, ct);

        if (productsWithDiscount.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                "Products with discount not found",    
                HttpStatusCode.NotFound
            );
        }
        
        List<ProductShortDto> productShortDtos = productsWithDiscount
            .Select(product => product.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds,
        CancellationToken ct)
    {
        IReadOnlyCollection<ProductShortProjection> recommendedProducts =
            await productRepository.GetRecommendedForPageAsync(pageSize, visitedCategoriesIds, ct);

        if (recommendedProducts.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                "Recommended products not found",
                HttpStatusCode.NotFound
            );
        }
        
        List<ProductShortDto> productShortDtos = recommendedProducts
            .Select(product => product.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }
}