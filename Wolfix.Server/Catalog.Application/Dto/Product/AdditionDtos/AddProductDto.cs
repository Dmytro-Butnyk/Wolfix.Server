using Catalog.Application.Dto.Product.Review;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed class AddProductDto
{
    public string Title { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string Status { get; init; }
    public Guid CategoryId { get; init; }
    public string ContentType { get; init; }
    public IFormFile Filestream { get; init; }
    public List<AddAttributeDto> Attributes { get; init; }
}