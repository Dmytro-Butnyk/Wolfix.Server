namespace Catalog.Domain.ValueObjects;

public sealed class AttributeAndUniqueValuesValueObject
{
    public Guid AttributeId { get; init; }
    public string Key { get; init; }
    public IReadOnlyCollection<string?>? Values { get; init; }
}