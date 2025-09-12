using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Projections.Category;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories;

internal sealed class CategoryRepository(CatalogContext context) :
    BaseRepository<CatalogContext, Category>(context), ICategoryRepository
{
    private readonly DbSet<Category> _categories = context.Categories;
    
    public async Task<IReadOnlyCollection<CategoryShortProjection>> GetAllParentCategoriesAsNoTrackingAsync(
        CancellationToken ct)
    {
        List<CategoryShortProjection> parentCategories = await _categories
            .AsNoTracking()
            .Where(category => category.Parent == null)
            .Select(category => new CategoryShortProjection(category.Id, category.Name))
            .ToListAsync(ct);
        
        return parentCategories;
    }

    public async Task<IReadOnlyCollection<CategoryShortProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(
        Guid parentId, CancellationToken ct)
    {
        List<CategoryShortProjection> childCategories = await _categories
            .AsNoTracking()
            .Where(category => category.Parent != null && category.Parent.Id == parentId)
            .Select(category => new CategoryShortProjection(category.Id, category.Name))
            .ToListAsync(ct);
        
        return childCategories;
    }

    public async Task<Category?> GetByIdWithProductAttributesAsNoTrackingAsync(Guid id, CancellationToken ct)
    {
        return await _categories
            .AsNoTracking()
            .Include("_productAttributes")
            .FirstOrDefaultAsync(category => category.Id == id, ct);
    }
}