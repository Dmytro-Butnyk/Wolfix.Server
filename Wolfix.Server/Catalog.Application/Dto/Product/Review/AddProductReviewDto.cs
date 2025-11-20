namespace Catalog.Application.Dto.Product.Review;

public sealed record AddProductReviewDto(string Title, string Text, uint Rating, Guid CustomerId);