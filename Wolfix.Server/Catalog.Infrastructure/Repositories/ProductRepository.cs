using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using LinqKit;


namespace Catalog.Infrastructure.Repositories;

internal sealed class ProductRepository(CatalogContext context)
    : BaseRepository<CatalogContext, Product>(context), IProductRepository
{
    private readonly DbSet<Product> _products = context.Products;

    public async Task<int> GetTotalCountAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        int totalCount = await _products
            .AsNoTracking()
            .CountAsync(ct);

        return totalCount;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetForPageAsync(int page, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        //todo
        return new List<ProductShortProjection>();
    }

    public async Task<int> GetTotalCountByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return await _products
            .AsNoTracking()
            .Where(product => ids.Contains(product.Id) && product.Status == ProductStatus.InStock)
            .CountAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetAllByCategoryIdForPageAsync(Guid childCategoryId,
        int page,
        int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        List<ProductShortProjection> productsByCategory = await _products
            .Include(p => p.Discount)
            .Include("_productMedias")
            .AsNoTracking()
            .Where(product => product.CategoryId == childCategoryId)
            .OrderBy(product => product.FinalPrice)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductShortProjection(product.Id, product.Title, product.AverageRating,
                product.Price, product.FinalPrice, product.Bonuses, product.MainPhotoUrl))
            .ToListAsync(ct);

        return productsByCategory;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        List<ProductShortProjection> productsWithDiscount = await _products
            .Include(p => p.Discount)
            .Include("_productMedias")
            .AsNoTracking()
            .Where(product => product.Discount != null && product.Discount.Status == DiscountStatus.Active)
            .OrderByDescending(product => product.Discount!.Percent)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductShortProjection(product.Id, product.Title, product.AverageRating,
                product.Price, product.FinalPrice, product.Bonuses, product.MainPhotoUrl))
            .ToListAsync(ct);

        return productsWithDiscount;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetRecommendedByCategoryIdAsync(Guid categoryId,
        int productsByCategorySize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return await _products
            .AsNoTracking()
            .Include(p => p.Discount)
            .Include("_productMedias")
            .Where(product => product.CategoryId == categoryId)
            .OrderBy(_ => EF.Functions.Random())
            .Take(productsByCategorySize)
            .Select(product => new ProductShortProjection(
                product.Id,
                product.Title,
                product.AverageRating,
                product.Price,
                product.FinalPrice,
                product.Bonuses,
                product.MainPhotoUrl))
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountWithDiscountAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        int totalCount = await _products
            .Include(p => p.Discount)
            .AsNoTracking()
            .Where(product => product.Discount != null && product.Discount.Status == DiscountStatus.Active)
            .CountAsync(ct);

        return totalCount;
    }

    public async Task<int> GetTotalCountByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        int totalCount = await _products
            .AsNoTracking()
            .Where(product => product.CategoryId == categoryId)
            .CountAsync(ct);

        return totalCount;
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetRandomAsync(int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return await _products
            .AsNoTracking()
            .Include(p => p.Discount)
            .Include("_productMedias")
            .OrderBy(p => EF.Functions.Random())
            .Take(pageSize)
            .Select(p => new ProductShortProjection(
                p.Id,
                p.Title,
                p.AverageRating,
                p.Price,
                p.FinalPrice,
                p.Bonuses,
                p.MainPhotoUrl))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductReviewProjection>> GetProductReviewsAsync(Guid productId, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _products
            .AsNoTracking()
            .Where(product => product.Id == productId)
            .Include("_reviews")
            .SelectMany(product => EF.Property<List<Review>>(product, "_reviews"))
            .OrderBy(review => review.CreatedAt)
            .Take(pageSize)
            .Select(review => new ProductReviewProjection(
                review.Id,
                review.Title,
                review.Text,
                review.Rating,
                review.ProductId,
                review.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductReviewProjection>> GetNextProductReviewsAsync(Guid productId,
        int pageSize, Guid lastId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _products
            .AsNoTracking()
            .Where(product => product.Id == productId)
            .Include("_reviews")
            .SelectMany(product => EF.Property<List<Review>>(product, "_reviews"))
            .Where(review => review.Id.CompareTo(lastId) > 0)
            .OrderBy(review => review.CreatedAt)
            .Take(pageSize)
            .Select(review => new ProductReviewProjection(
                review.Id,
                review.Title,
                review.Text,
                review.Rating,
                review.ProductId,
                review.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<Guid>> GetAllMediaIdsByCategoryProductsAsync(Guid categoryId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _products
            .AsNoTracking()
            .Where(product => product.CategoryId == categoryId)
            .Include("_productMedias")
            .SelectMany(product => EF.Property<List<ProductMedia>>(product, "_productMedias"))
            .Select(media => media.MediaId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetBySearchQueryAsync(string searchQuery,
        int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        const double threshold = 0.30; // можно варьировать 0.25..0.45

        return await _products
            .Include(p => p.Discount)
            .Include("_productMedias")
            .AsNoTracking()
            .Where(p =>
                EF.Functions.TrigramsSimilarity(p.Title, searchQuery) > threshold)
            .OrderByDescending(p => EF.Functions.TrigramsSimilarity(p.Title, searchQuery))
            .Take(pageSize)
            .Select(product => new ProductShortProjection(
                product.Id,
                product.Title,
                product.AverageRating,
                product.Price,
                product.FinalPrice,
                product.Bonuses,
                product.MainPhotoUrl))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetBySearchQueryAndCategoryAsync(
        Guid categoryId, string searchQuery, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        const double threshold = 0.30; // можно варьировать 0.25..0.45

        return await _products
            .Include(p => p.Discount)           
            .Include("_productMedias")
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId &&
                        // использовать TrigramsSimilarity (возвращает double 0..1)
                        EF.Functions.TrigramsSimilarity(p.Title, searchQuery) > threshold)
            .OrderByDescending(p => EF.Functions.TrigramsSimilarity(p.Title, searchQuery))
            .Take(pageSize)
            .Select(product => new ProductShortProjection(
                product.Id,
                product.Title,
                product.AverageRating,
                product.Price,
                product.FinalPrice,
                product.Bonuses,
                product.MainPhotoUrl))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<Guid>> GetByAttributesFiltrationAsNoTrackingAsync(
        IReadOnlyCollection<(Guid AttributeId, string Value)> filters, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var predicate = PredicateBuilder.New<ProductAttributeValue>(false);

        foreach (var f in filters)
        {
            var temp = f; // обязательно, чтобы захват переменной корректно
            predicate = predicate.Or(pa =>
                EF.Property<Guid>(pa, "CategoryAttributeId") == temp.AttributeId &&
                EF.Property<string>(pa, "Value") == temp.Value
            );
        }

        var query = _products
            .AsNoTracking()
            .SelectMany(p => EF.Property<List<ProductAttributeValue>>(p, "_productAttributeValues"))
            .Where(predicate)
            .Take(pageSize)
            .Select(pa => (
                pa.Product.Id
            ));

        return await query.Distinct().ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ProductShortProjection>> GetShortProductsByIdsAsNoTrackingAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var products = await _products
            .Include("_productMedias")
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Select(p => new ProductShortProjection(
                p.Id,
                p.Title,
                p.AverageRating,
                p.Price,
                p.FinalPrice,
                p.Bonuses,
                p.MainPhotoUrl
            ))
            .ToListAsync(ct);

        return products;
    }

    public async Task<IReadOnlyCollection<AttributeAndUniqueValuesValueObject>> GetAttributesAndUniqueValuesAsync(
        Guid childCategory, IReadOnlyCollection<Guid> attributeIds, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var result = await _products
            .AsNoTracking()
            .Include("_productAttributeValues")
            .Where(p => p.CategoryId == childCategory)
            .SelectMany(p => EF.Property<List<ProductAttributeValue>>(p, "_productAttributeValues"))
            .Where(pav => attributeIds.Contains(pav.CategoryAttributeId))
            .GroupBy(pav => new { pav.CategoryAttributeId, pav.Key })
            .Select(g => new AttributeAndUniqueValuesValueObject
            {
                AttributeId = g.Key.CategoryAttributeId,
                Key = g.Key.Key,
                Values = g.Select(x => x.Value).Distinct().ToList()
            })
            .ToListAsync(ct);

        return result;
    }
}