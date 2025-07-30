using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Domain.Catalog.Interfaces;

public interface IProductRepository
    : IBaseRepository<Product>, IPaginationRepository<ProductShortProjection>
{
    Task<IReadOnlyCollection<ProductShortProjection>> GetAllByCategoryIdAsNoTrackingAsync(Guid childCategoryId,
        CancellationToken ct);
    
    Task<IReadOnlyCollection<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);

    Task<IReadOnlyCollection<ProductShortProjection>> GetRecommendedByCategoryIdAsync(Guid categoryId,
        int productsByCategorySize, CancellationToken ct);
}