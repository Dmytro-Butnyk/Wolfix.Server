using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

internal sealed class Review : BaseEntity
{
    public string Title { get; private set; }
    
    public string Text { get; private set; }
    
    public uint Rating { get; private set; }
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public Product Product { get; private set; }
    
    //todo: user id
    
    private Review() { }
    
    private Review(string title, string text, uint rating, Product product)
    {
        Title = title;
        Text = text;
        Rating = rating;
        Product = product;
    }

    internal static Result<Review> Create(string title, string text, uint rating, Product product)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Review>.Failure($"{nameof(title)} is required");
        }
        
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result<Review>.Failure($"{nameof(text)} is required");
        }

        if (rating is > 5 or < 1)
        {
            return Result<Review>.Failure($"{nameof(rating)} must be between 1 and 5");
        }

        var review = new Review(title, text, rating, product);
        return Result<Review>.Success(review, HttpStatusCode.Created);
    }

    internal VoidResult SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return VoidResult.Failure($"{nameof(title)} is required");
        }
        
        Title = title;
        return VoidResult.Success();
    }
    
    internal VoidResult SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return VoidResult.Failure($"{nameof(text)} is required");
        }
        
        Text = text;
        return VoidResult.Success();
    }
    
    internal VoidResult SetRating(uint rating)
    {
        if (rating is > 5 or < 1)
        {
            return VoidResult.Failure($"{nameof(rating)} must be between 1 and 5");
        }

        Rating = rating;
        return VoidResult.Success();
    }
}

public record ReviewInfo(string Title, string Text, uint Rating, DateTime CreatedAt);