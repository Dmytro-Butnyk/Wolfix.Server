using Catalog.Application.Dto.Product;
using Catalog.Domain.Projections.Product;

namespace Catalog.Application.Mapping.Product;

internal static class ProductMapper
{
    public static ProductShortDto ToShortDto(this ProductShortProjection productShortProjection)
    {
        return new ProductShortDto(productShortProjection.Id, productShortProjection.Title,
            productShortProjection.AverageRating,
            productShortProjection.Price, productShortProjection.FinalPrice, productShortProjection.Bonuses,
            productShortProjection.MainPhoto);
    }
}