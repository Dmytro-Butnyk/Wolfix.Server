using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.Entities;

internal sealed class FavoriteItem : BaseEntity
{
    public Guid CustomerId { get; private set; }
    
    public string PhotoUrl { get; private set; }
    
    public string Title { get; private set; }
    
    public double? AverageRating { get; private set; }
    
    public decimal Price { get; private set; }
    
    public decimal? FinalPrice { get; private set; }
    
    public uint Bonuses { get; private set; }

    private FavoriteItem() { }

    private FavoriteItem(Guid customerId, string photoUrl, string title,
        decimal price, uint bonuses, double? averageRating = null, decimal? finalPrice = null)
    {
        CustomerId = customerId;
        PhotoUrl = photoUrl;
        Title = title;
        AverageRating = averageRating;
        Price = price;
        FinalPrice = finalPrice;
        Bonuses = bonuses;
    }

    public static Result<FavoriteItem> Create(Guid customerId, string photoUrl, string title,
        decimal price, uint bonuses, double? averageRating = null, decimal? finalPrice = null)
    {
        if (customerId == Guid.Empty)
        {
            return Result<FavoriteItem>.Failure($"{nameof(customerId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Result<FavoriteItem>.Failure($"{nameof(photoUrl)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<FavoriteItem>.Failure($"{nameof(title)} cannot be null or empty");
        }

        if (price <= 0)
        {
            return Result<FavoriteItem>.Failure($"{nameof(price)} cannot be less than or equal to zero");
        }

        if (bonuses <= 0)
        {
            return Result<FavoriteItem>.Failure($"{nameof(bonuses)} cannot be less than or equal to zero");
        }

        FavoriteItem favoriteItem = new(customerId, photoUrl, title, price, bonuses, averageRating, finalPrice);
        return Result<FavoriteItem>.Success(favoriteItem);
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

public sealed record FavoriteItemInfo(Guid CustomerId, string PhotoUrl, string Title, double? AverageRating,
    decimal Price, decimal? FinalPrice, uint Bonuses);