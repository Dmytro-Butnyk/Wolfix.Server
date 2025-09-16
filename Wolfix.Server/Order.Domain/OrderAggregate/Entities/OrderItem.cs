using Shared.Domain.Entities;
using Shared.Domain.Models;

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
    
    private OrderItem() { }

    private OrderItem(Guid productId, string photoUrl, string title, uint quantity, decimal price, Order order)
    {
        ProductId = productId;
        PhotoUrl = photoUrl;
        Title = title;
        Quantity = quantity;
        Price = price;
        Order = order;
        OrderId = order.Id;
    }

    public static Result<OrderItem> Create(Guid productId, string photoUrl, string title, uint quantity,
        decimal price, Order order)
    {
        if (Guid.Empty == productId)
        {
            return Result<OrderItem>.Failure($"{nameof(productId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Result<OrderItem>.Failure($"{nameof(photoUrl)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<OrderItem>.Failure($"{nameof(title)} cannot be null or empty");
        }

        if (quantity <= 0)
        {
            return Result<OrderItem>.Failure($"{nameof(quantity)} must be positive");
        }
        
        if (price <= 0)
        {
            return Result<OrderItem>.Failure($"{nameof(price)} must be positive");
        }

        return Result<OrderItem>.Success(new(productId, photoUrl, title, quantity, price, order));
    }
    
    public static explicit operator OrderItemInfo(OrderItem orderItem)
        => new(orderItem.Id, orderItem.ProductId, orderItem.PhotoUrl, orderItem.Title,
            orderItem.Quantity, orderItem.Price);
}

public sealed record OrderItemInfo(Guid Id, Guid ProductId, string PhotoUrl, string Title, uint Quantity, decimal Price);