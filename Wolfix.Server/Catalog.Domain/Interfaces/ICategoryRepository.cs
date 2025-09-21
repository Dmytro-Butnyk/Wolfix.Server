using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Projections.Category;
using Shared.Domain.Interfaces;

namespace Catalog.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<bool> IsExistAsync(string name, CancellationToken ct, bool ignoreCase = true);
    
    Task<IReadOnlyCollection<CategoryFullProjection>> GetAllParentCategoriesAsNoTrackingAsync(CancellationToken ct);
    Task<IReadOnlyCollection<CategoryFullProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(Guid parentId,
        CancellationToken ct);
    Task<Category?> GetByIdWithProductAttributesAsNoTrackingAsync(Guid id, CancellationToken ct);
    
    Task<IReadOnlyCollection<CategoryShortProjection>> GetAllChildCategoriesAsync(CancellationToken ct);
}