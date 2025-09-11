using Catalog.Application.Dto.Product.Review;

namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed record AddProductDto(
    string Title, 
    string Description,
    decimal Price,
    string Status,
    Guid CategoryId,
    AddMediaDto Media,
    IReadOnlyCollection<AddAttributeDto> Attributes);