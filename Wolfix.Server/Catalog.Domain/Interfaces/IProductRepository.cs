using Catalog.Domain.ProductAggregate;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Shared.Domain.Interfaces;

namespace Catalog.Domain.Interfaces;

public interface IProductRepository
    : IBaseRepository<Product>, IPaginationRepository<ProductShortProjection>
{
    Task<IReadOnlyCollection<ProductShortProjection>> GetAllByCategoryIdForPageAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);

    Task<IReadOnlyCollection<ProductShortProjection>> GetRecommendedByCategoryIdAsync(Guid categoryId,
        int productsByCategorySize, CancellationToken ct);
    
    Task<int> GetTotalCountWithDiscountAsync(CancellationToken ct);
    
    Task<int> GetTotalCountByCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<IReadOnlyCollection<ProductShortProjection>> GetRandomAsync(int randomSkip, int pageSize, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductReviewProjection>> GetProductReviewsAsync(Guid productId, int pageSize, CancellationToken ct);
    Task<IReadOnlyCollection<ProductReviewProjection>> GetNextProductReviewsAsync(Guid productId, int pageSize, Guid lastId, CancellationToken ct);
}