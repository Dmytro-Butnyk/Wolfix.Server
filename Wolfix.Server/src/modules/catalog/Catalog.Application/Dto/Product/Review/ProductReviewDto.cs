namespace Catalog.Application.Dto.Product.Review;

public sealed record ProductReviewDto(Guid Id, string Title, string Text, uint Rating,
    Guid ProductId, DateTime CreatedAt);