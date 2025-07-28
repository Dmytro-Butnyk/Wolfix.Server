using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<ProductShortDto>>> GetAllByCategoryIdAsync(Guid childCategoryId, CancellationToken ct);
    Task<Result<IEnumerable<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize, CancellationToken ct);
}