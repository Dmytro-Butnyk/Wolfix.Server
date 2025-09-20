using Seller.Domain.SellerApplicationAggregate;
using Shared.Domain.Interfaces;

namespace Seller.Domain.Interfaces;

public interface ISellerApplicationRepository : IBaseRepository<SellerApplication>
{
    Task<IReadOnlyCollection<SellerApplication>> GetByAccountIdAsNoTrackingAsync(Guid accountId, CancellationToken ct);
}