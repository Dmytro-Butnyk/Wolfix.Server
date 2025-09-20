namespace Catalog.Application.Dto.Product.AttributesFiltrationDto;

public sealed class FiltrationAttributeDto
{
    public Guid AttributeId { get; init; }
    public string Key { get; init; }
    public string Value { get; init; }
}