using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.ProductAggregate;
using Wolfix.Infrastructure.Shared.Database;

namespace Wolfix.Infrastructure.Catalog.Repositories;

public sealed class ProductRepository(WolfixStoreContext context) 
    : BaseRepository<Product>(context), IProductRepository
{
    //todo: product repository
}