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
    public async Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct)
    {
        const string cacheKey = "all_parent_categories";

        List<CategoryShortDto> parentCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
        {
            IReadOnlyCollection<CategoryShortProjection> parentCategories =
                await categoryRepository.GetAllParentCategoriesAsNoTrackingAsync(ctx);

            return parentCategories
                .Select(category => category.ToShortDto())
                .ToList();
        }, ct, TimeSpan.FromMinutes(20));

        if (parentCategoriesDto.Count == 0)
        {
            return Result<IReadOnlyCollection<CategoryShortDto>>.Failure(
                "No parent categories found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(parentCategoriesDto);
    }

    public async Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct)
    {
        //todo: cacheKey = $"child_categories_by_parent_{parentId}"
        const string cacheKey = "all_child_categories";
        
        List<CategoryShortDto> childCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
        {
            IReadOnlyCollection<CategoryShortProjection> childCategories =
                await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ctx);

            return childCategories
                .Select(category => category.ToShortDto())
                .ToList();
        }, ct, TimeSpan.FromMinutes(20));
        
        if (childCategoriesDto.Count == 0)
        {
            return Result<IReadOnlyCollection<CategoryShortDto>>.Failure(
                "No child categories found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(childCategoriesDto);
    }
}