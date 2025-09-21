using Catalog.Domain.ValueObjects;
using Shared.Domain.Models;

namespace Catalog.Domain.Interfaces.DomainServices;

public interface ICategoryDomainService
{
     Task<IReadOnlyCollection<AttributeAndUniqueValuesValueObject>>
          GetAttributesAndUniqueValuesAsync(Guid childCategory,
              IReadOnlyCollection<Guid> attributeIds,
              CancellationToken ct); 
}