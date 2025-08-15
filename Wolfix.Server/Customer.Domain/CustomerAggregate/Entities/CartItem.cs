using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.Entities;

internal sealed class CartItem : BaseEntity
{
    public Guid CustomerId { get; private set; }
    
    public string PhotoUrl { get; private set; }
    
    public string Title { get; private set; }
    
    public decimal Price { get; private set; }
    
    private CartItem() { }
    
    private CartItem(Guid customerId, string photoUrl, string title,
        decimal price)
    {
        CustomerId = customerId;
        PhotoUrl = photoUrl;
        Title = title;
        Price = price;
    }

    public static Result<CartItem> Create(Guid customerId, string photoUrl, string title,
        decimal price)
    {
        if (customerId == Guid.Empty)
        {
            return Result<CartItem>.Failure($"{nameof(customerId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Result<CartItem>.Failure($"{nameof(photoUrl)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<CartItem>.Failure($"{nameof(title)} cannot be null or empty");
        }

        if (price <= 0)
        {
            return Result<CartItem>.Failure($"{nameof(price)} cannot be less than or equal to zero");
        }

        CartItem cartItem = new(customerId, photoUrl, title, price);
        return Result<CartItem>.Success(cartItem);
    }

    public VoidResult SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            return VoidResult.Failure($"{nameof(customerId)} cannot be empty");
        }
        
        return VoidResult.Success();
    }
}

public sealed record CartItemInfo(Guid CustomerId, string PhotoUrl, string Title,
    decimal Price);