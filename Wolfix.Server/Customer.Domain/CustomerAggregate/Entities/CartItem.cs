using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.Entities;

internal sealed class CartItem : BaseEntity
{
    //todo: maybe setters
    public string PhotoUrl { get; private set; }
    
    public string Title { get; private set; }
    
    public decimal Price { get; private set; }
    
    public Customer Customer { get; private set; }
    public Guid CustomerId { get; private set; }
    
    private CartItem() { }
    
    private CartItem(Customer customer, string photoUrl, string title,
        decimal price)
    {
        Customer = customer;
        CustomerId = customer.Id;
        PhotoUrl = photoUrl;
        Title = title;
        Price = price;
    }

    public static Result<CartItem> Create(Customer customer, string photoUrl, string title,
        decimal price)
    {
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

        CartItem cartItem = new(customer, photoUrl, title, price);
        return Result<CartItem>.Success(cartItem);
    }

    public static explicit operator CartItemInfo(CartItem cartItem)
        => new(cartItem.CustomerId, cartItem.PhotoUrl, cartItem.Title, cartItem.Price);
}

public sealed record CartItemInfo(Guid CustomerId, string PhotoUrl, string Title,
    decimal Price);