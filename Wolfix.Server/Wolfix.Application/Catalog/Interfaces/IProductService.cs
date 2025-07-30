using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Shared.Dto;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Interfaces;

public interface IProductService
{
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(int page, int pageSize,
        Guid childCategoryId,
        CancellationToken ct);
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct);
}