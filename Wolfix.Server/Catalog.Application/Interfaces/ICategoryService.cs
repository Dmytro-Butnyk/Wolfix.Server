using Catalog.Application.Dto.Category;
using Catalog.Application.Dto.Category.Requests;
using Catalog.Application.Dto.Category.Responses;
using Shared.Domain.Models;

namespace Catalog.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllParentCategoriesAsync(CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<CategoryShortDto>>> GetAllChildCategoriesByParentAsync(Guid parentId,
        CancellationToken ct);
    
    Task<IReadOnlyCollection<CategoryShortDto>> GetAllChildCategoriesAsync(CancellationToken ct);
    
    Task<VoidResult> AddParentAsync(AddParentCategoryDto request, CancellationToken ct);
    
    Task<VoidResult> AddChildAsync(AddChildCategoryDto request, Guid parentId, CancellationToken ct);
    
    Task<Result<ParentCategoryDto>> ChangeParentAsync(ChangeParentCategoryDto request, Guid categoryId, CancellationToken ct);
    
    Task<Result<ChildCategoryDto>> ChangeChildAsync(ChangeChildCategoryDto request, Guid childCategoryId, CancellationToken ct);
    
    Task<VoidResult> AddAttributeAsync(AddCategoryAttributeDto request, Guid childCategoryId, CancellationToken ct);
    
    Task<VoidResult> AddVariantAsync(AddCategoryVariantDto request, Guid childCategoryId, CancellationToken ct);
    
    Task<VoidResult> DeleteCategoryAsync(Guid categoryId, CancellationToken ct);
    
    Task<VoidResult> DeleteAttributeAsync(Guid childCategoryId, Guid attributeId, CancellationToken ct);
    
    Task<VoidResult> DeleteVariantAsync(Guid childCategoryId, Guid variantId, CancellationToken ct);
}