using Customer.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Customer.Domain.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer.Domain.CustomerAggregate.Customer>
{
    Task<IReadOnlyCollection<FavoriteItemProjection>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct);
}