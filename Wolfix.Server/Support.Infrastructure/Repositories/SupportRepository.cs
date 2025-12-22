using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Interfaces;

namespace Support.Infrastructure.Repositories;

internal sealed class SupportRepository(SupportContext context)
    : BaseRepository<SupportContext, Domain.Entities.Support>(context), ISupportRepository
{
    private readonly DbSet<Domain.Entities.Support> _supports = context.Set<Domain.Entities.Support>();
    
    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _supports
            .AsNoTracking()
            .Where(support => support.AccountId == accountId)
            .Select(support => support.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsExistAsync(string firstName, string lastName, string middleName, CancellationToken ct)
    {
        return await _supports
            .AsNoTracking()
            .Where(support => support.FullName.FirstName == firstName
            && support.FullName.LastName == lastName
            && support.FullName.MiddleName == middleName)
            .AnyAsync(ct);
    }
}