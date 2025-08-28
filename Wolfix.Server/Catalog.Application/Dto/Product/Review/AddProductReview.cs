namespace Catalog.Application.Dto.Product.Review;

public sealed record AddProductReview(string Title, string Text, uint Rating, Guid CustomerId);