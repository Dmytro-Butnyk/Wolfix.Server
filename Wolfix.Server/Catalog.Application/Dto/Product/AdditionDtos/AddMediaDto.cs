using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;

namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed class AddMediaDto
{
    public Guid ProductId { get; init; }
    public IFormFile? Media { get; init; }
    public string ContentType { get; init; }
}