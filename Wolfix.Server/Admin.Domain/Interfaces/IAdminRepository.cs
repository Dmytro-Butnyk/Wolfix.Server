using Shared.Domain.Interfaces;

namespace Admin.Domain.Interfaces;

public interface IAdminRepository : IBaseRepository<AdminAggregate.Admin>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, bool isSuperAdmin, CancellationToken ct);
}