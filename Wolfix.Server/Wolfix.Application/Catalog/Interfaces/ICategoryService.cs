using Wolfix.Application.Catalog.Dto.Category;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct);
    Task<Result<IEnumerable<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId, CancellationToken ct);
}