using Catalog.Application.Dto.Category;
using Shared.Domain.Models;

namespace Catalog.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct);
    Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct);
}