using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.Entities;

internal sealed class CartItem : BaseEntity
{
    //todo: maybe setters
    public string PhotoUrl { get; private set; }
    
    public string Title { get; private set; }
    
    public decimal PriceWithDiscount { get; private set; }
    
    public Customer Customer { get; private set; }
    public Guid CustomerId { get; private set; }
    
    private CartItem() { }
    
    private CartItem(Customer customer, string photoUrl, string title,
        decimal priceWithDiscount)
    {
        Customer = customer;
        CustomerId = customer.Id;
        PhotoUrl = photoUrl;
        Title = title;
        PriceWithDiscount = priceWithDiscount;
    }

    public static Result<CartItem> Create(Customer customer, string photoUrl, string title,
        decimal priceWithDiscount)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Result<CartItem>.Failure($"{nameof(photoUrl)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<CartItem>.Failure($"{nameof(title)} cannot be null or empty");
        }

        if (priceWithDiscount <= 0)
        {
            return Result<CartItem>.Failure($"{nameof(priceWithDiscount)} cannot be less than or equal to zero");
        }

        CartItem cartItem = new(customer, photoUrl, title, priceWithDiscount);
        return Result<CartItem>.Success(cartItem);
    }

    public static explicit operator CartItemInfo(CartItem cartItem)
        => new(cartItem.Id, cartItem.CustomerId, cartItem.PhotoUrl, cartItem.Title, cartItem.PriceWithDiscount);
}

public sealed record CartItemInfo(Guid Id, Guid CustomerId, string PhotoUrl, string Title,
    decimal PriceWithDiscount);