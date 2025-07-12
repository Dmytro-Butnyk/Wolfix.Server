using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

public sealed class Review : BaseEntity
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

        if (rating > 5)
        {
            return Result<Review>.Failure($"{nameof(rating)} must be less than 6");
        }

        var review = new Review(title, text, rating, product);
        return Result<Review>.Success(review, HttpStatusCode.Created);
    }
}