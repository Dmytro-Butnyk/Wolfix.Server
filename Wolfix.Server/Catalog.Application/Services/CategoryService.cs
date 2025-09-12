using System.Net;
using Catalog.Application.Dto.Category;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Application.Dto.Category.Responses;
using Catalog.Application.Interfaces;
using Catalog.Application.Mapping.Category;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Projections.Category;
using Shared.Application.Interfaces;
using Shared.Domain.Models;

namespace Catalog.Application.Services;

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
        
        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(parentCategoriesDto);
    }

    public async Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct)
    {
        if (!await categoryRepository.IsExistAsync(parentId, ct))
        {
            return Result<IReadOnlyCollection<CategoryShortDto>>.Failure(
                $"Parent category with id: {parentId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        var cacheKey = $"child_categories_by_parent_{parentId}";
        
        List<CategoryShortDto> childCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
        {
            IReadOnlyCollection<CategoryShortProjection> childCategories =
                await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ctx);

            return childCategories
                .Select(category => category.ToShortDto())
                .ToList();
        }, ct, TimeSpan.FromMinutes(20));
        
        return Result<IReadOnlyCollection<CategoryShortDto>>.Success(childCategoriesDto);
    }

    public async Task<VoidResult> AddParentAsync(AddParentCategoryDto request, CancellationToken ct)
    {
        if (await categoryRepository.IsExistAsync(request.Name, ct))
        {
            return VoidResult.Failure(
                $"Category with name: {request.Name} already exists",
                HttpStatusCode.Conflict
            );
        }
        
        Result<Category> createCategoryResult = Category.Create(request.Name, request.Description);

        if (!createCategoryResult.IsSuccess)
        {
            return VoidResult.Failure(createCategoryResult);
        }
        
        Category category = createCategoryResult.Value!;

        await categoryRepository.AddAsync(category, ct);
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> AddChildAsync(AddChildCategoryDto request, Guid parentId, CancellationToken ct)
    {
        Category? parentCategory = await categoryRepository.GetByIdAsync(parentId, ct, "Parent");

        if (parentCategory is null)
        {
            return VoidResult.Failure(
                $"Parent category with id: {parentId} not found",
                HttpStatusCode.NotFound
            );
        }

        if (parentCategory.Parent != null)
        {
            return VoidResult.Failure(
                $"Parent category with id: {parentId} already has a parent",
                HttpStatusCode.Conflict
            );
        }
        
        if (await categoryRepository.IsExistAsync(request.Name, ct))
        {
            return VoidResult.Failure(
                $"Category with name: {request.Name} already exists",
                HttpStatusCode.Conflict
            );
        }
        
        Result<Category> createCategoryResult = Category.Create(request.Name, request.Description, parentCategory);

        if (!createCategoryResult.IsSuccess)
        {
            return VoidResult.Failure(createCategoryResult);
        }
        
        Category category = createCategoryResult.Value!;

        VoidResult addAttributesResult = category.AddProductAttributes(request.AttributeKeys);

        if (!addAttributesResult.IsSuccess)
        {
            return VoidResult.Failure(addAttributesResult);
        }
        
        VoidResult addVariantsResult = category.AddProductVariants(request.VariantKeys);

        if (!addVariantsResult.IsSuccess)
        {
            return VoidResult.Failure(addVariantsResult);
        }
        
        await categoryRepository.AddAsync(category, ct);
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<Result<ParentCategoryDto>> ChangeParentAsync(ChangeParentCategoryDto request, Guid categoryId, CancellationToken ct)
    {
        Category? parentCategory = await categoryRepository.GetByIdAsync(categoryId, ct, "Parent");
        
        if (parentCategory is null)
        {
            return Result<ParentCategoryDto>.Failure(
                $"Category with id: {categoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (parentCategory.Parent != null)
        {
            return Result<ParentCategoryDto>.Failure(
                $"Category with id: {categoryId} is not a parent category",
                HttpStatusCode.Conflict
            );
        }
        
        if (await categoryRepository.IsExistAsync(request.Name, ct))
        {
            return Result<ParentCategoryDto>.Failure(
                $"Category with name: {request.Name} already exists",
                HttpStatusCode.Conflict
            );
        }
        
        bool isNothingChanged = true;

        if (IsNotTheSame(parentCategory.Name, request.Name))
        {
            VoidResult changeCategoryName = parentCategory.ChangeName(request.Name);

            if (!changeCategoryName.IsSuccess)
            {
                return Result<ParentCategoryDto>.Failure(changeCategoryName);
            }
            isNothingChanged = false;
        }
        
        if ((parentCategory.Description == null && request.Description != null)
            || IsNotTheSame(parentCategory.Description, request.Description))
        {
            VoidResult changeCategoryDescription = parentCategory.ChangeDescription(request.Description);

            if (!changeCategoryDescription.IsSuccess)
            {
                return Result<ParentCategoryDto>.Failure(changeCategoryDescription);
            }
            isNothingChanged = false;
        }

        if (isNothingChanged)
        {
            return Result<ParentCategoryDto>.Failure("The same data provided");
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        ParentCategoryDto dto = new(request.Name, request.Description);
        return Result<ParentCategoryDto>.Success(dto);
    }
    
    private bool IsNotTheSame(string current, string newOne)
        => !current.Equals(newOne, StringComparison.OrdinalIgnoreCase);
}