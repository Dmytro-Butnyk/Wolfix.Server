using Shared.Domain.Interfaces;

namespace Support.Domain.Interfaces;

public interface ISupportRepository : IBaseRepository<Entities.Support>
{
    Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct);
    Task<bool> IsExistAsync(string firstName, string lastName, string middleName, CancellationToken ct);
}