using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Infrastructure.Shared.Repositories;

namespace Wolfix.Infrastructure.Catalog.Repositories;

public sealed class CategoryRepository(WolfixStoreContext context) :
    BaseRepository<Category>(context), ICategoryRepository
{
    //todo: category repository
}