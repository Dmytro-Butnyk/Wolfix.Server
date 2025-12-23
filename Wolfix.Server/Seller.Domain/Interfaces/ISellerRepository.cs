using Seller.Domain.Projections.Seller;
using Shared.Domain.Interfaces;

namespace Seller.Domain.Interfaces;

public interface ISellerRepository : IBaseRepository<SellerAggregate.Seller>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct);
    
    Task<SellerProjection?> GetProfileInfoAsync(Guid sellerId, CancellationToken ct);
    
    Task<int> GetTotalCountAsync(CancellationToken ct);
    
    Task<IReadOnlyCollection<SellerForAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct);
}