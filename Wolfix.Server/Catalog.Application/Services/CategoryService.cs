using System.Net;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Application.Dto.Category.Responses;
using Catalog.Application.Dto.Category.Responses.CategoryAttributesAndUniqueValues;
using Catalog.Application.Mapping.Category;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Projections.Category;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Shared.Application.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.Services;

public sealed class CategoryService(
    ICategoryRepository categoryRepository,
    ProductDomainService productDomainService,
    CategoryDomainService categoryDomainService,
    EventBus eventBus,
    IAppCache appCache)
{
    public async Task<Result<IReadOnlyCollection<CategoryFullDto>>> GetAllParentCategoriesAsync(CancellationToken ct)
    {
        const string cacheKey = "all_parent_categories";

        List<CategoryFullDto> parentCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
        {
            IReadOnlyCollection<CategoryFullProjection> parentCategories =
                await categoryRepository.GetAllParentCategoriesAsNoTrackingAsync(ctx);

            return parentCategories
                .Select(category => category.ToFullDto())
                .ToList();
        }, ct, TimeSpan.FromMinutes(20));
        
        return Result<IReadOnlyCollection<CategoryFullDto>>.Success(parentCategoriesDto);
    }

    public async Task<Result<IReadOnlyCollection<CategoryFullDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct)
    {
        if (!await categoryRepository.IsExistAsync(parentId, ct))
        {
            return Result<IReadOnlyCollection<CategoryFullDto>>.Failure(
                $"Parent category with id: {parentId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        var cacheKey = $"child_categories_by_parent_{parentId}";
        
        List<CategoryFullDto> childCategoriesDto = await appCache.GetOrCreateAsync(cacheKey, async ctx =>
        {
            IReadOnlyCollection<CategoryFullProjection> childCategories =
                await categoryRepository.GetAllChildCategoriesByParentAsNoTrackingAsync(parentId, ctx);

            return childCategories
                .Select(category => category.ToFullDto())
                .ToList();
        }, ct, TimeSpan.FromMinutes(20));
        
        return Result<IReadOnlyCollection<CategoryFullDto>>.Success(childCategoriesDto);
    }

    public async Task<IReadOnlyCollection<CategoryShortDto>> GetAllChildCategoriesAsync(CancellationToken ct)
    {
        IReadOnlyCollection<CategoryShortProjection> childCategories =
            await categoryRepository.GetAllChildCategoriesAsync(ct);
        
        return childCategories
            .Select(category => category.ToShortDto())
            .ToList();
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

        var @event = new AddPhotoForNewCategory
        {
            FileData = request.Photo
        };
        
        Result<CreatedMediaDto> createImageResult =
            await eventBus.PublishWithSingleResultAsync<AddPhotoForNewCategory, CreatedMediaDto>(@event, ct);

        if (createImageResult.IsFailure)
        {
            return VoidResult.Failure(createImageResult);
        }

        CreatedMediaDto createdBlobResource = createImageResult.Value!;
        
        Result<Category> createCategoryResult = Category.Create(createdBlobResource.BlobResourceId, createdBlobResource.Url,
            request.Name, request.Description);

        if (!createCategoryResult.IsSuccess)
        {
            return VoidResult.Failure(createCategoryResult);
        }
        
        Category category = createCategoryResult.Value!;

        await categoryRepository.AddAsync(category, ct);
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    //todo: проблема
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

        if (!parentCategory.IsParent)
        {
            return VoidResult.Failure(
                $"Category with id: {parentId} already has a parent",
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
        
        var @event = new AddPhotoForNewCategory
        {
            FileData = request.Photo
        };
        
        Result<CreatedMediaDto> createImageResult =
            await eventBus.PublishWithSingleResultAsync<AddPhotoForNewCategory, CreatedMediaDto>(@event, ct);

        if (createImageResult.IsFailure)
        {
            return VoidResult.Failure(createImageResult);
        }

        CreatedMediaDto createdBlobResource = createImageResult.Value!;
        
        Result<Category> createCategoryResult = Category.Create(createdBlobResource.BlobResourceId, createdBlobResource.Url,
            request.Name, request.Description, parentCategory);

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
        
        if (!parentCategory.IsParent)
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

        VoidResult changeCategoryName = parentCategory.ChangeName(request.Name);

        if (!changeCategoryName.IsSuccess) 
        {
            return Result<ParentCategoryDto>.Failure(changeCategoryName);
        }
        
        isNothingChanged = false;
        
        VoidResult changeCategoryDescription = parentCategory.ChangeDescription(request.Description);

        if (!changeCategoryDescription.IsSuccess)
        {
            return Result<ParentCategoryDto>.Failure(changeCategoryDescription);
        }
            
        isNothingChanged = false;

        if (isNothingChanged)
        {
            return Result<ParentCategoryDto>.Failure("The same data provided");
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        ParentCategoryDto dto = new(request.Name, request.Description);
        return Result<ParentCategoryDto>.Success(dto);
    }

    public async Task<Result<ChildCategoryDto>> ChangeChildAsync(ChangeChildCategoryDto request, Guid childCategoryId, CancellationToken ct)
    {
        Category? childCategory = await categoryRepository.GetByIdAsync(childCategoryId, ct, "Parent");
        
        if (childCategory is null)
        {
            return Result<ChildCategoryDto>.Failure(
                $"Child category with id: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!childCategory.IsChild)
        {
            return Result<ChildCategoryDto>.Failure(
                $"Category with id: {childCategoryId} is not a child category",
                HttpStatusCode.Conflict
            );
        }
        
        if (await categoryRepository.IsExistAsync(request.Name, ct))
        {
            return Result<ChildCategoryDto>.Failure(
                $"Child category with name: {request.Name} already exists",
                HttpStatusCode.Conflict
            );
        }
        
        bool isNothingChanged = true;

        VoidResult changeCategoryName = childCategory.ChangeName(request.Name);
            
        if (!changeCategoryName.IsSuccess)
        {
            return Result<ChildCategoryDto>.Failure(changeCategoryName);
        }
        
        isNothingChanged = false;
        
        VoidResult changeCategoryDescription = childCategory.ChangeDescription(request.Description);

        if (!changeCategoryDescription.IsSuccess)
        {
            return Result<ChildCategoryDto>.Failure(changeCategoryDescription);
        }
        
        isNothingChanged = false;

        if (isNothingChanged)
        {
            return Result<ChildCategoryDto>.Failure("The same data provided");
        }

        await categoryRepository.SaveChangesAsync(ct);

        ChildCategoryDto dto = new(request.Name, request.Description);
        return Result<ChildCategoryDto>.Success(dto);
    }

    public async Task<VoidResult> AddAttributeAsync(AddCategoryAttributeDto request, Guid childCategoryId, CancellationToken ct)
    {
        Category? childCategory = await categoryRepository.GetByIdAsync(childCategoryId, ct, 
            "Parent", "_productAttributes");
        
        if (childCategory is null)
        {
            return VoidResult.Failure(
                $"Child category with id: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!childCategory.IsChild)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} is not a child category",
                HttpStatusCode.Conflict
            );
        }
        
        VoidResult addAttributeResult = childCategory.AddProductAttribute(request.Key);

        if (addAttributeResult.IsFailure)
        {
            return VoidResult.Failure(addAttributeResult);
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        //todo: кидать уведомление продавцу о том что нужно добавить значение для аттрибута
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> AddVariantAsync(AddCategoryVariantDto request, Guid childCategoryId, CancellationToken ct)
    {
        Category? childCategory = await categoryRepository.GetByIdAsync(childCategoryId, ct, 
            "Parent", "_productVariants");
        
        if (childCategory is null)
        {
            return VoidResult.Failure(
                $"Child category with id: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!childCategory.IsChild)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} is not a child category",
                HttpStatusCode.Conflict
            );
        }
        
        VoidResult addVariantResult = childCategory.AddProductVariant(request.Key);
        
        if (addVariantResult.IsFailure)
        {
            return VoidResult.Failure(addVariantResult);
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        //todo: кидать уведомление продавцу о том что нужно добавить значение для варианта
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsync(categoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure(
                $"Category with id: {categoryId} not found",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<Guid> allMediaIdsOfCategoryProducts =
            await productDomainService.GetAllMediaIdsByCategoryProducts(categoryId, ct);

        if (allMediaIdsOfCategoryProducts.Count > 0)
        {
            await eventBus.PublishWithoutResultAsync(new CategoryAndProductsDeleted
            {
                MediaIds = allMediaIdsOfCategoryProducts
            }, ct);
        }

        categoryRepository.Delete(category, ct);
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();   
    }

    public async Task<VoidResult> DeleteAttributeAsync(Guid childCategoryId, Guid attributeId, CancellationToken ct)
    {
        Category? childCategory = await categoryRepository.GetByIdAsync(childCategoryId, ct,
            "Parent", "_productAttributes");
        
        if (childCategory is null)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!childCategory.IsChild)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} is not a child category",
                HttpStatusCode.Conflict
            );
        }

        VoidResult deleteAttributeResult = childCategory.RemoveProductAttribute(attributeId);

        if (!deleteAttributeResult.IsSuccess)
        {
            return VoidResult.Failure(deleteAttributeResult);
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteVariantAsync(Guid childCategoryId, Guid variantId, CancellationToken ct)
    {
        Category? childCategory = await categoryRepository.GetByIdAsync(childCategoryId, ct,
            "Parent", "_productVariants");
        
        if (childCategory is null)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!childCategory.IsChild)
        {
            return VoidResult.Failure(
                $"Category with id: {childCategoryId} is not a child category",
                HttpStatusCode.Conflict
            );
        }

        VoidResult deleteVariantResult = childCategory.RemoveProductVariant(variantId);
        
        if (!deleteVariantResult.IsSuccess)
        {
            return VoidResult.Failure(deleteVariantResult);
        }
        
        await categoryRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<Result<IReadOnlyCollection<AttributeAndUniqueValuesDto>>> GetCategoryAttributesAndUniqueValuesAsync(Guid childCategoryId, CancellationToken ct)
    {
        Category? childCategory =
            await categoryRepository.GetByIdAsNoTrackingAsync(
                childCategoryId,
                ct,
                "_productAttributes",
                "Parent");
        
        if (childCategory is null)
        {
            return Result<IReadOnlyCollection<AttributeAndUniqueValuesDto>>
                .Failure($"Category with id:{childCategoryId} not found", HttpStatusCode.NotFound);
        }
        
        if (!childCategory.IsChild)
        {
            return Result<IReadOnlyCollection<AttributeAndUniqueValuesDto>>
                .Failure($"Category with id: {childCategoryId} is not a child category");
        }
        
        IReadOnlyCollection<Guid> attributeIds = childCategory.ProductAttributes
            .Select(attribute => attribute.Id)
            .ToList();

        IReadOnlyCollection<AttributeAndUniqueValuesValueObject> attributesAndUniqueValues =
            await categoryDomainService
                .GetAttributesAndUniqueValuesAsync(childCategoryId, attributeIds, ct);

        IReadOnlyCollection<AttributeAndUniqueValuesDto>  attributeAndUniqueValuesDtos =
            attributesAndUniqueValues.Select(anu => 
                new AttributeAndUniqueValuesDto
                {
                    AttributeId = anu.AttributeId,
                    Key = anu.Key,
                    Values = anu.Values
                }).ToList();
        
        return Result<IReadOnlyCollection<AttributeAndUniqueValuesDto>>.Success(attributeAndUniqueValuesDtos);   
    }

    public async Task<Result<IReadOnlyCollection<CategoryAttributeDto>>> GetAllAttributesByCategoryAsync(Guid childId, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsNoTrackingAsync(childId, ct, 
            "_productAttributes", "Parent");

        if (category is null)
        {
            return Result<IReadOnlyCollection<CategoryAttributeDto>>.Failure(
                $"Category with id: {childId} not found",
                HttpStatusCode.NotFound
            );
        }

        if (!category.IsChild)
        {
            return Result<IReadOnlyCollection<CategoryAttributeDto>>.Failure($"Category with id: {childId}is not a child category");
        }

        IReadOnlyCollection<CategoryAttributeDto> dto = category.ProductAttributes
            .Select(attribute => new CategoryAttributeDto(attribute.Id, attribute.Key))
            .ToList();
        
        return Result<IReadOnlyCollection<CategoryAttributeDto>>.Success(dto);
    }
}