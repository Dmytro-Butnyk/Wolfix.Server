using Seller.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Seller.Domain.Interfaces;

public interface ISellerRepository : IBaseRepository<SellerAggregate.Seller>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct);
    
    Task<SellerProjection?> GetProfileInfoAsync(Guid sellerId, CancellationToken ct);
}