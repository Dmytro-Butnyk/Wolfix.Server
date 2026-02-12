using Shared.Domain.Interfaces;
using Support.Domain.Projections;

namespace Support.Domain.Interfaces;

public interface ISupportRepository : IBaseRepository<Entities.Support>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct);
    Task<bool> IsExistAsync(string firstName, string lastName, string middleName, CancellationToken ct);
    
    Task<int> GetTotalCountAsync(CancellationToken ct);
    
    Task<IReadOnlyCollection<SupportForAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct);
}