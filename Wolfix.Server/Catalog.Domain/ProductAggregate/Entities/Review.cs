using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.Entities;

internal sealed class Review : BaseEntity
{
    public string Title { get; private set; }
    
    public string Text { get; private set; }
    
    public uint Rating { get; private set; }
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public Product Product { get; private set; }
    public Guid ProductId { get; private set; }
    
    //todo: user id
    
    private Review() { }
    
    private Review(string title, string text, uint rating, Product product)
    {
        Title = title;
        Text = text;
        Rating = rating;
        Product = product;
        ProductId = product.Id;
    }

    internal static Result<Review> Create(string title, string text, uint rating, Product product)
    {
        if (IsTextInvalid(title, out var titleErrorMessage))
        {
            return Result<Review>.Failure(titleErrorMessage);
        }
        
        if (IsTextInvalid(text, out var textErrorMessage))
        {
            return Result<Review>.Failure(textErrorMessage);
        }
        
        if (IsRatingInvalid(rating, out var ratingErrorMessage))
        {
            return Result<Review>.Failure(ratingErrorMessage);
        }

        var review = new Review(title, text, rating, product);
        return Result<Review>.Success(review, HttpStatusCode.Created);
    }

    internal VoidResult SetTitle(string title)
    {
        if (IsTextInvalid(title, out var titleErrorMessage))
        {
            return VoidResult.Failure(titleErrorMessage);
        }
        
        Title = title;
        return VoidResult.Success();
    }
    
    internal VoidResult SetText(string text)
    {
        if (IsTextInvalid(text, out var textErrorMessage))
        {
            return VoidResult.Failure(textErrorMessage);
        }
        
        Text = text;
        return VoidResult.Success();
    }
    
    internal VoidResult SetRating(uint rating)
    {
        if (IsRatingInvalid(rating, out var ratingErrorMessage))
        {
            return VoidResult.Failure(ratingErrorMessage);
        }

        Rating = rating;
        return VoidResult.Success();
    }

    #region validation
    private static bool IsTextInvalid(string text, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            errorMessage = $"{nameof(text)} is required";
            return true;
        }

        errorMessage = string.Empty;
        return false;
    }

    private static bool IsRatingInvalid(uint rating, out string errorMessage)
    {
        if (rating is > 5 or < 1)
        {
            errorMessage = $"{nameof(rating)} must be between 1 and 5";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    #endregion
    
    public static explicit operator ReviewInfo(Review review)
        => new(review.Id, review.Title, review.Text, review.Rating, review.CreatedAt);
}

public record ReviewInfo(Guid Id, string Title, string Text, uint Rating, DateTime CreatedAt);