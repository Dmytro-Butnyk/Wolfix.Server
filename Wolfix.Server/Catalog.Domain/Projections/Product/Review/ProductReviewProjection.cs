using Shared.Domain.Projections;

namespace Catalog.Domain.Projections.Product.Review;

public sealed record ProductReviewProjection(Guid Id, string Title, string Text, uint Rating,
    Guid ProductId, DateTime CreatedAt): BaseProjection(Id);