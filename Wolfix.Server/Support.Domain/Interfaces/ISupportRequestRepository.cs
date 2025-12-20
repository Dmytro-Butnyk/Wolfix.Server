using Shared.Domain.Interfaces;
using Support.Domain.Entities;
using Support.Domain.Projections;

namespace Support.Domain.Interfaces;

public interface ISupportRequestRepository : IBaseRepository<SupportRequest>
{
    Task<IReadOnlyCollection<SupportRequestShortProjection>> GetAllPendingAsync(CancellationToken ct);
}