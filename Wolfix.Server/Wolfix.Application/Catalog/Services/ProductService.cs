using System.Net;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Product;
using Wolfix.Application.Shared.Dto;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct)
    {
        var errorMessage = $"Products by category: {childCategoryId} not found";
        
        int totalCount = await productRepository.GetTotalCountByCategoryAsync(childCategoryId, ct);

        if (totalCount == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                errorMessage,
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<ProductShortProjection> productsByCategory =
            await productRepository.GetAllByCategoryIdForPageAsync(childCategoryId, page, pageSize, ct);

        if (productsByCategory.Count == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                errorMessage,
                HttpStatusCode.NotFound
            );
        }
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        List<ProductShortDto> productShortDtos = productsByCategory
            .Select(product => product.ToShortDto())
            .ToList();
        
        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);
        
        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        int totalCount = await productRepository.GetTotalCountWithDiscountAsync(ct);

        if (totalCount == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(
                "Products with discount not found",    
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
        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        List<ProductShortDto> productShortDtos = productsWithDiscount
            .Select(product => product.ToShortDto())
            .ToList();

        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);
        
        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct)
    {
        List<ProductShortProjection> recommendedProducts = new(pageSize);
        
        int productsByCategorySize = pageSize / visitedCategoriesIds.Count;
        int remainder = pageSize % visitedCategoriesIds.Count;
        
        for (var i = 0; i < visitedCategoriesIds.Count; ++i)
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

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRandomProductsAsync(int pageSize,
        CancellationToken ct)
    {
        int productCount = await productRepository.GetTotalCountAsync(ct);

        if (productCount == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                "Products list is empty",    
                HttpStatusCode.NotFound
            );
        }
        
        var random = new Random();
        int randomSkip = random.Next(1, productCount);

        List<ProductShortProjection> products = (await productRepository
            .GetRandomAsync(randomSkip, pageSize, ct))
            .ToList();

        if (products.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                "Products not found",    
                HttpStatusCode.NotFound
            );
        }
        
        List<ProductShortDto> randomProducts = products
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(randomProducts);
    }
}