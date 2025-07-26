using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections;
using Wolfix.Domain.Shared;
using Wolfix.Infrastructure.Shared.Repositories;

namespace Wolfix.Infrastructure.Catalog.Repositories;

internal sealed class CategoryRepository(WolfixStoreContext context) :
    BaseRepository<Category>(context), ICategoryRepository
{
    private readonly DbSet<Category> _categories = context.Categories;
    
    public async Task<IEnumerable<CategoryShortProjection>> GetAllParentCategoriesAsNoTrackingAsync(
        CancellationToken ct)
    {
        List<CategoryShortProjection> parentCategories = await _categories
            .AsNoTracking()
            .Where(category => category.Parent == null)
            .Select(category => new CategoryShortProjection(category.Id, category.Name))
            .ToListAsync(ct);
        
        return parentCategories;
    }

    public async Task<IEnumerable<CategoryShortProjection>> GetAllChildCategoriesByParentAsNoTrackingAsync(
        Guid parentId, CancellationToken ct)
    {
        List<CategoryShortProjection> childCategories = await _categories
            .AsNoTracking()
            .Where(category => category.Parent != null && category.Parent.Id == parentId)
            .Select(category => new CategoryShortProjection(category.Id, category.Name))
            .ToListAsync(ct);
        
        return childCategories;
    }
}