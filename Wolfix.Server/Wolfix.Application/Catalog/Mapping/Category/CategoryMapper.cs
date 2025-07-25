using Wolfix.Application.Catalog.Dto.Category;
using Wolfix.Domain.Catalog.Projections;

namespace Wolfix.Application.Catalog.Mapping.Category;

internal static class CategoryMapper
{
    public static CategoryShortDto ToShortDto(this CategoryShortProjection categoryShortProjection)
    {
        return new CategoryShortDto(categoryShortProjection.Id, categoryShortProjection.Name);
    }
}