using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.Review;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Catalog.Application.Interfaces;

public interface IProductService
{
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRandomProductsAsync(int pageSize, CancellationToken ct);
    
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct);
    
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<ProductReviewDto>>> GetProductReviewsAsync(Guid productId, CancellationToken ct);
}