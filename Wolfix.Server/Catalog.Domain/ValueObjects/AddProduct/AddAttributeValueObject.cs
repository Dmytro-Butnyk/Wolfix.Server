namespace Catalog.Domain.ValueObjects.AddProduct;

public sealed record AddAttributeValueObject(
    Guid ProductAttributeId,
    string Value);
