using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.Projections;
using Wolfix.Domain.Catalog.Projections.Category;
using Wolfix.Domain.Shared;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Domain.Catalog.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IReadOnlyCollection<CategoryShortProjection>> GetAllParentCategoriesAsNoTrackingAsync(CancellationToken ct);
    Task<IReadOnlyCollection<CategoryShortProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(Guid parentId,
        CancellationToken ct);
}