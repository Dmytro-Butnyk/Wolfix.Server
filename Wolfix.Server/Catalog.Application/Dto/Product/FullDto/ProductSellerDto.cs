namespace Catalog.Application.Dto.Product.FullDto;

public sealed class ProductSellerDto
{
    public required Guid SellerId { get; init; }
    public required string SellerFullName { get; init; }
    public required string? SellerPhotoUrl { get; init; }
}