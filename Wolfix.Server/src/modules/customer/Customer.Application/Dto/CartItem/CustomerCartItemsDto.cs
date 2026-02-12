namespace Customer.Application.Dto.CartItem;

public sealed record CustomerCartItemsDto
{
    public required IReadOnlyCollection<CartItemDto> Items { get; init; }
    
    public required Guid CustomerId { get; init; }
    
    public required decimal BonusesAmount { get; init; }
    
    public required decimal TotalCartPriceWithoutBonuses { get; init; }
}