using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Domain.Catalog.Projections.Product;

namespace Wolfix.Application.Catalog.Mapping.Product;

internal static class ProductMapper
{
    public static ProductShortDto ToShortDto(this ProductShortProjection productShortProjection)
    {
        return new ProductShortDto(productShortProjection.Id, productShortProjection.Title,
            productShortProjection.AverageRating,
            productShortProjection.Price, productShortProjection.FinalPrice, productShortProjection.Bonuses);
    }
}