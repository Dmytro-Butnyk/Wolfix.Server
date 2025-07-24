using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
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
}