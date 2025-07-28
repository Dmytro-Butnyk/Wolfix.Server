using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Domain.Catalog.Interfaces;

public interface IProductRepository
    : IBaseRepository<Product>, IPaginationRepository<ProductShortProjection>
{
    Task<IEnumerable<ProductShortProjection>> GetAllByCategoryIdAsNoTrackingAsync(Guid childCategoryId,
        CancellationToken ct);
    
    Task<IEnumerable<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize, CancellationToken ct);
    
    Task<int> GetProductCountAsync(CancellationToken ct);

    Task<IEnumerable<ProductShortProjection>> GetRandom(int randomSkip, int pageSize, CancellationToken ct);
}