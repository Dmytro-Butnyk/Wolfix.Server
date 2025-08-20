namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed record ProductAdditionDto(
    string Title, 
    string Description,
    decimal Price,
    string Status,
    Guid CategoryId,
    IReadOnlyCollection<MediaDto> Media);