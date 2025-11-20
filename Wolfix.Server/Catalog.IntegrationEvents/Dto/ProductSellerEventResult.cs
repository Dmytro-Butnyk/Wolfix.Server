namespace Catalog.IntegrationEvents.Dto;

public sealed class ProductSellerEventResult
{
    public required Guid SellerId { get; init; }
    public required string SellerFullName { get; init; }
    public required string? SellerPhotoUrl { get; init; }
}