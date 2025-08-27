using System.Net;
using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.Review;
using Catalog.Application.Interfaces;
using Catalog.Application.Mapping.Product;
using Catalog.Application.Mapping.Product.Review;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Catalog.Application.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct)
    {
        //todo: добавить проверку на существование категории через доменный сервис(или событие) и кинуть нот фаунт если нету
        
        int totalCount = await productRepository.GetTotalCountByCategoryAsync(childCategoryId, ct);

        if (totalCount == 0)
        {
            PaginationDto<ProductShortDto> dto = new(1, 1, 0, new List<ProductShortDto>());
            return Result<PaginationDto<ProductShortDto>>.Success(dto);
        }
        
        IReadOnlyCollection<ProductShortProjection> productsByCategory =
            await productRepository.GetAllByCategoryIdForPageAsync(childCategoryId, page, pageSize, ct);
        
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
            PaginationDto<ProductShortDto> dto = new(1, 1, 0, new List<ProductShortDto>());
            return Result<PaginationDto<ProductShortDto>>.Success(dto);
        }
        
        IReadOnlyCollection<ProductShortProjection> productsWithDiscount =
            await productRepository.GetForPageWithDiscountAsync(page, pageSize, ct);
        
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
        //todo: добавить проверку на существование категорий через доменный сервис(или событие) и кинуть нот фаунт если нету
        
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

    public async Task<Result<IReadOnlyCollection<ProductReviewDto>>> GetProductReviewsAsync(Guid productId, CancellationToken ct)
    {
        if (!await productRepository.IsExistAsync(productId, ct))
        {
            return Result<IReadOnlyCollection<ProductReviewDto>>.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<ProductReviewProjection> productReviews =
            await productRepository.GetProductReviewsAsync(productId, ct);

        List<ProductReviewDto> productReviewsDto = productReviews
            .Select(productReview => productReview.ToDto())
            .ToList();
        
        return Result<IReadOnlyCollection<ProductReviewDto>>.Success(productReviewsDto);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRandomProductsAsync(int pageSize,
        CancellationToken ct)
    {
        int productCount = await productRepository.GetTotalCountAsync(ct);

        if (productCount == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Success([]);
        }
        
        var random = new Random();
        int randomSkip = random.Next(1, productCount);

        IReadOnlyCollection<ProductShortProjection> randomProducts = await productRepository.GetRandomAsync(randomSkip, pageSize, ct);

        if (randomProducts.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Success([]);
        }
        
        List<ProductShortDto> randomProductsDto = randomProducts
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(randomProductsDto);
    }
}