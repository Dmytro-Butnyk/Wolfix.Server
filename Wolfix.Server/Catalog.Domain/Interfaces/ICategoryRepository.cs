using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Projections.Category;
using Shared.Domain.Interfaces;

namespace Catalog.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IReadOnlyCollection<CategoryShortProjection>> GetAllParentCategoriesAsNoTrackingAsync(CancellationToken ct);
    Task<IReadOnlyCollection<CategoryShortProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(Guid parentId,
        CancellationToken ct);
}