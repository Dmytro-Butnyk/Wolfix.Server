using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Interfaces;

public interface IProductService
{
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetAllByCategoryIdAsync(Guid childCategoryId,
        CancellationToken ct);
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct);
}