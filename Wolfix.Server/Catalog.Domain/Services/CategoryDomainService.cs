using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.DomainServices;
using Catalog.Domain.ValueObjects;
using Shared.Domain.Models;

namespace Catalog.Domain.Services;

public sealed class CategoryDomainService(
    IProductRepository productRepository)
    : ICategoryDomainService
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