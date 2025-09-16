using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Dto.Product.FullDto;

public sealed class ProductFullDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public decimal FinalPrice { get; init; }
    public required string Status { get; init; }
    public uint Bonuses { get; init; }
    public double? AverageRating { get; init; }
    public required IReadOnlyCollection<ProductVariantDto> Variants { get; init; }
    public required IReadOnlyCollection<ProductCategoryDto> Categories  { get; init; }
    public required IReadOnlyCollection<ProductMediaDto> Medias { get; init; }
    public required IReadOnlyCollection<ProductAttributeDto> Attributes { get; init; }
    //todo: seller information
}