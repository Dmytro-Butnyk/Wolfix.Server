using Catalog.Application.Dto.Category.Responses;
using Catalog.Domain.Projections.Category;

namespace Catalog.Application.Mapping.Category;

internal static class CategoryMapper
{
    public static CategoryShortDto ToShortDto(this CategoryShortProjection categoryShortProjection)
        => new(categoryShortProjection.Id, categoryShortProjection.Name);

    public static CategoryFullDto ToFullDto(this CategoryFullProjection projection)
        => new(projection.Id, projection.Name, projection.PhotoUrl);
}