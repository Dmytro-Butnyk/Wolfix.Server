using Catalog.Domain.Interfaces;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Services;

public sealed class CategoryDomainService(IProductRepository productRepository)
{
    public async Task<IReadOnlyCollection<AttributeAndUniqueValuesValueObject>> 
        GetAttributesAndUniqueValuesAsync(
            Guid childCategory,
            IReadOnlyCollection<Guid> attributeIds,
            CancellationToken ct)
    {
        IReadOnlyCollection<AttributeAndUniqueValuesValueObject> result = 
            await productRepository.GetAttributesAndUniqueValuesAsync(childCategory, attributeIds, ct);
        
        return result;
    }
}