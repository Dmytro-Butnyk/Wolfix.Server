using Catalog.Domain.ProductAggregate;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Catalog.Domain.ValueObjects;
using Shared.Domain.Interfaces;

namespace Catalog.Domain.Interfaces;

public interface IProductRepository
    : IBaseRepository<Product>, IPaginationRepository<ProductShortProjection>
{
    Task<int> GetTotalCountByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetAllByCategoryIdForPageAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);

    Task<IReadOnlyCollection<ProductShortProjection>> GetRecommendedByCategoryIdAsync(Guid categoryId,
        int productsByCategorySize, CancellationToken ct);
    
    Task<int> GetTotalCountWithDiscountAsync(CancellationToken ct);
    
    Task<int> GetTotalCountByCategoryAsync(Guid categoryId, CancellationToken ct);
    
    Task<int> GetTotalCountBySellerCategoryAsync(Guid sellerId, Guid categoryId, CancellationToken ct);

    Task<IReadOnlyCollection<ProductShortProjection>> GetRandomAsync(int pageSize, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductReviewProjection>> GetProductReviewsAsync(Guid productId, int pageSize, CancellationToken ct);
    Task<IReadOnlyCollection<ProductReviewProjection>> GetNextProductReviewsAsync(Guid productId, int pageSize, Guid lastId, CancellationToken ct);
    
    Task<IReadOnlyCollection<Guid>> GetAllMediaIdsByCategoryProductsAsync(Guid categoryId, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetBySearchQueryAsync(string searchQuery, int pageSize,
        CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetBySearchQueryAndCategoryAsync(Guid categoryId, string searchQuery, int pageSize,
        CancellationToken ct);
    
    Task<IReadOnlyCollection<Guid>> GetByAttributesFiltrationAsNoTrackingAsync (
        IReadOnlyCollection<(Guid AttributeId, string Value)> attributeFilters,
        decimal? minPrice,
        decimal? maxPrice,
        int pageSize,
        int pageNumber,
        int skipCount,
        CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetShortProductsByIdsAsNoTrackingAsync (IReadOnlyCollection<Guid> ids, CancellationToken ct);
    
    Task<IReadOnlyCollection<AttributeAndUniqueValuesValueObject>> GetAttributesAndUniqueValuesAsync(Guid childCategory, IReadOnlyCollection<Guid> attributeIds, CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetAllBySellerCategoryForPageAsync(Guid sellerId, Guid categoryId, int page, int pageSize, CancellationToken ct);
}