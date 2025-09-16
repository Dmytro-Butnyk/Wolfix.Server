using Shared.Domain.Entities;

namespace Order.Domain.OrderAggregate.Entities;

internal sealed class OrderItem : BaseEntity
{
    public string PhotoUrl { get; private set; }
    
    public string Title { get; private set; }
    
    public uint Quantity { get; private set; }
    
    public decimal Price { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public Order Order { get; private set; }
    public Guid OrderId { get; private set; }
    
    public static explicit operator OrderItemInfo(OrderItem orderItem)
        => new(orderItem.Id, orderItem.ProductId, orderItem.PhotoUrl, orderItem.Title,
            orderItem.Quantity, orderItem.Price);
}

public sealed record OrderItemInfo(Guid Id, Guid ProductId, string PhotoUrl, string Title, uint Quantity, decimal Price);