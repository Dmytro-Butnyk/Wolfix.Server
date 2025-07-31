using System.Net;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Product;
using Wolfix.Application.Shared.Dto;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(int page, int pageSize,
        Guid childCategoryId, CancellationToken ct)
    {
        int totalCount = await productRepository.GetTotalCountAsync(ct);

        if (totalCount == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                "Products not found",
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<ProductShortProjection> productsByCategory =
            await productRepository.GetAllByCategoryIdAsNoTrackingAsync(childCategoryId, ct);

        if (productsByCategory.Count == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                $"Products by category: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        List<ProductShortDto> productShortDtos = productsByCategory
            .Select(product => product.ToShortDto())
            .ToList();
        
        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);
        
        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        int totalCount = await productRepository.GetTotalCountAsync(ct);

        if (totalCount == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                "Products not found",    
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<ProductShortProjection> productsWithDiscount =
            await productRepository.GetForPageWithDiscountAsync(page, pageSize, ct);

        if (productsWithDiscount.Count == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                "Products with discount not found",    
                HttpStatusCode.NotFound
            );
        }
        
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        List<ProductShortDto> productShortDtos = productsWithDiscount
            .Select(product => product.ToShortDto())
            .ToList();

        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);
        
        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct)
    {
        //todo: указать сразу количество эллементов списка
        List<ProductShortProjection> recommendedProducts = [];
        
        int productsByCategorySize = pageSize / visitedCategoriesIds.Count;
        int remainder = pageSize % visitedCategoriesIds.Count;
        
        for (int i = 0; i < visitedCategoriesIds.Count; ++i)
        {
            int count = productsByCategorySize + (i < remainder ? 1 : 0);
            Guid id = visitedCategoriesIds[i];
            
            IReadOnlyCollection<ProductShortProjection> recommendedByCategory =
                await productRepository.GetRecommendedByCategoryIdAsync(id, count, ct);
            recommendedProducts.AddRange(recommendedByCategory);
        }

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