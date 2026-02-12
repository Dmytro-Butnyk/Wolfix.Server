using Admin.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Admin.Domain.Interfaces;

public interface IAdminRepository : IBaseRepository<AdminAggregate.Admin>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, bool isSuperAdmin, CancellationToken ct);
    
    Task<int> GetBasicAdminsTotalCountAsync(CancellationToken ct);
    
    Task<IReadOnlyCollection<BasicAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct);
}