using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.Projections;
using Wolfix.Domain.Shared;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Domain.Catalog.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<CategoryShortProjection>> GetAllParentCategoriesAsNoTrackingAsync(CancellationToken ct);
    Task<IEnumerable<CategoryShortProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(Guid parentId, CancellationToken ct);
}