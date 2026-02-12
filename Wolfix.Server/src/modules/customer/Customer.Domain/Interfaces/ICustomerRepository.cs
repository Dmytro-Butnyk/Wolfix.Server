using Customer.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Customer.Domain.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer.Domain.CustomerAggregate.Customer>
{
    Task<IReadOnlyCollection<FavoriteItemProjection>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<IReadOnlyCollection<CartItemProjection>> GetCartItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct);
    
    Task<CustomerProjection?> GetProfileInfoAsync(Guid customerId, CancellationToken ct);
}