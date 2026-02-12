using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed class AddProductDto
{
    public string Title { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string Status { get; init; }
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
    public string ContentType { get; init; }
    public IFormFile? Media { get; init; }
    
    [FromForm(Name = "Attributes")]
    public required string AttributesJson { get; init; }

    public List<AddAttributeDto> Attributes =>
        string.IsNullOrEmpty(AttributesJson)
            ? new List<AddAttributeDto>()
            : JsonSerializer.Deserialize<List<AddAttributeDto>>(
                AttributesJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
}