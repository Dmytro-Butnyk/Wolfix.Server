using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ValueObjects.AddProduct;
using Shared.Domain.Models;

namespace Catalog.Domain.Interfaces.DomainServices;

public interface IProductDomainService
{
    Task<VoidResult> AddProductAsync(
        string title,
        string description,
        decimal price,
        ProductStatus status,
        Guid categoryId,
        AddMediaValueObject media,
        IReadOnlyCollection<AddAttributeValueObject> attributes,
        CancellationToken ct);
}