using Catalog.Application.Dto.Product.Review;
using Catalog.Domain.Projections.Product.Review;

namespace Catalog.Application.Mapping.Product.Review;

internal static class ProductReviewMapper
{
    public static ProductReviewDto ToDto(this ProductReviewProjection projection)
        => new(projection.Id, projection.Title, projection.Text, projection.Rating,
            projection.ProductId, projection.CreatedAt);
}