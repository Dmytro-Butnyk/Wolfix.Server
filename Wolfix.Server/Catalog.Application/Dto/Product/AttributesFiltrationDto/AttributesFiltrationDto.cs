namespace Catalog.Application.Dto.Product.AttributesFiltrationDto;

public sealed class AttributesFiltrationDto
{
    public Guid CategoryId { get; init; }
    public required IReadOnlyCollection<FiltrationAttributeDto> FiltrationAttribute { get; init; }    
}

