using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Domain.Catalog.ProductAggregate.Enums;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Infrastructure.Shared.Repositories;

namespace Wolfix.Infrastructure.Catalog.Repositories;

internal sealed class ProductRepository(WolfixStoreContext context) 
    : BaseRepository<Product>(context), IProductRepository
{
    private readonly DbSet<Product> _products = context.Products;
    
    //todo: product repository
    public async Task<int> GetTotalCountAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        int totalCount = await _products.CountAsync(ct);

        return totalCount;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetForPageAsync(int page, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        //todo
        return new List<ProductShortProjection>();
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetAllByCategoryIdAsNoTrackingAsync(Guid childCategoryId, int pageSize,
        Guid? cursor, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var query = _products
            .Include(p => p.Discount)
            .AsNoTracking()
            .Where(product => product.CategoryId == childCategoryId);

        if (cursor.HasValue)
        {
            query = query.Where(product => product.Id > cursor.Value);
        }

        List<ProductShortProjection> productsByCategory = await query
            .OrderBy(product => product.Id)
            .Take(pageSize)
            .Select(product => new ProductShortProjection(product.Id, product.Title, product.AverageRating,
                product.Price, product.FinalPrice, product.Bonuses))
            .ToListAsync(ct);

        return productsByCategory;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        List<ProductShortProjection> productsWithDiscount = await _products
            .Include(p => p.Discount)
            .AsNoTracking()
            .Where(product => product.Discount != null && product.Discount.Status == DiscountStatus.Active)
            .OrderByDescending(product => product.Discount!.Percent)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductShortProjection(product.Id, product.Title, product.AverageRating,
                product.Price, product.FinalPrice, product.Bonuses))
            .ToListAsync(ct);

        return productsWithDiscount;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetRecommendedByCategoryIdAsync(Guid categoryId, int productsByCategorySize, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return await _products
            .AsNoTracking()
            .Include(p => p.Discount)
            .Where(product => product.CategoryId == categoryId)
            .OrderBy(_ => EF.Functions.Random())
            .Take(productsByCategorySize)
            .Select(product => new ProductShortProjection(product.Id, product.Title, product.AverageRating,
                product.Price, product.FinalPrice, product.Bonuses))
            .ToListAsync(ct);
    }
}