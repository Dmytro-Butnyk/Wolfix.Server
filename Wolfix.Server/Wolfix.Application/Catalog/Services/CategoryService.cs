using System.Net;
using Wolfix.Application.Catalog.Dto.Category;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Category;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections;
using Wolfix.Domain.Catalog.Projections.Category;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<IEnumerable<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct)
    {
        List<CategoryShortProjection> parentCategories =
            (await categoryRepository.GetAllParentCategoriesAsNoTrackingAsync(ct))
            .ToList();

        if (parentCategories.Count == 0)
        {
            return Result<IEnumerable<CategoryShortDto>>.Failure(
                "No parent categories found",
                HttpStatusCode.NotFound
            );
        }

        List<CategoryShortDto> parentCategoriesDto = parentCategories
            .Select(category => category.ToShortDto())
            .ToList();

        return Result<IEnumerable<CategoryShortDto>>.Success(parentCategoriesDto);
    }

    public async Task<Result<IEnumerable<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(
        Guid parentId, CancellationToken ct)
    {
        List<CategoryShortProjection> childCategories =
            (await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ct))
            .ToList();

        if (childCategories.Count == 0)
        {
            return Result<IEnumerable<CategoryShortDto>>.Failure(
                "No child categories found",
                HttpStatusCode.NotFound
            );
        }
        
        List<CategoryShortDto> childCategoriesDto = childCategories
            .Select(category => category.ToShortDto())
            .ToList();
        
        return Result<IEnumerable<CategoryShortDto>>.Success(childCategoriesDto);
    }
}