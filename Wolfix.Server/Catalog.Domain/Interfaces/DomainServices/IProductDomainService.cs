using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ValueObjects.AddProduct;
using Shared.Domain.Models;

namespace Catalog.Domain.Interfaces.DomainServices;

public interface IProductDomainService
{
    Task<Result<Guid>> AddProductAsync(
        string title,
        string description,
        decimal price,
        ProductStatus status,
        Guid categoryId,
        IReadOnlyCollection<AddAttributeValueObject> attributes,
        CancellationToken ct);

    Task<IReadOnlyCollection<Guid>> GetAllMediaIdsByCategoryProducts(Guid categoryId, CancellationToken ct);
    
    Task<VoidResult> AddAttributeToProductsAsync(Guid childCategoryId, string key, CancellationToken ct);
    
    Task<VoidResult> AddVariantToProductsAsync(Guid childCategoryId, string key, CancellationToken ct);
}