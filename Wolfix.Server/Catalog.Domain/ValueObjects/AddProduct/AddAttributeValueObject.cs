namespace Catalog.Domain.ValueObjects.AddProduct;

public sealed record AddAttributeValueObject(
    Guid CategoryAttributeId,
    string Value);
