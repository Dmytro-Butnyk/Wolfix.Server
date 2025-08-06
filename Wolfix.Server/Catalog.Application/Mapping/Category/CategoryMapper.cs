using Catalog.Application.Dto.Category;
using Catalog.Domain.Projections.Category;

namespace Catalog.Application.Mapping.Category;

internal static class CategoryMapper
{
    public static CategoryShortDto ToShortDto(this CategoryShortProjection categoryShortProjection)
    {
        return new CategoryShortDto(categoryShortProjection.Id, categoryShortProjection.Name);
    }
}