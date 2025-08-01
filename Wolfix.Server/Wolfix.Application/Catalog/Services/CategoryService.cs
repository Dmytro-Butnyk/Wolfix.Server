using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Wolfix.Application.Catalog.Dto.Category;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Category;
using Wolfix.Application.Shared.Interfaces;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections.Category;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class CategoryService(
    ICategoryRepository categoryRepository,
    IAppCache appCache
    ) : ICategoryService
{
    //todo: внедрить кэш
    public async Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct)
    {
        IReadOnlyCollection<CategoryShortProjection> parentCategories = await categoryRepository.GetAllParentCategoriesAsNoTrackingAsync(ct);

        if (parentCategories.Count == 0)
        {
            return Result<IReadOnlyCollection<CategoryShortDto>>.Failure(
                "No parent categories found",
                HttpStatusCode.NotFound
            );
        }

        List<CategoryShortDto> parentCategoriesDto = parentCategories
            .Select(category => category.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(parentCategoriesDto);
    }

    public async Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct)
    {
        IReadOnlyCollection<CategoryShortProjection> childCategories =
            await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ct);

        if (childCategories.Count == 0)
        {
            return Result<IReadOnlyCollection<CategoryShortDto>>.Failure(
                "No child categories found",
                HttpStatusCode.NotFound
            );
        }
        
        List<CategoryShortDto> childCategoriesDto = childCategories
            .Select(category => category.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(childCategoriesDto);
    }
}