namespace Catalog.Application.Dto.Category.Responses.CategoryAttributesAndUniqueValues;

public sealed class AttributeAndUniqueValuesDto
{
    public Guid AttributeId { get; init; }
    public string Key { get; init; }
    public IReadOnlyCollection<string> Values { get; init; }   
}