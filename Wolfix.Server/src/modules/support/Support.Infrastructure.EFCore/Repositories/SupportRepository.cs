using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Interfaces;
using Support.Domain.Projections;

namespace Support.Infrastructure.Repositories;

internal sealed class SupportRepository(SupportContext context)
    : BaseRepository<SupportContext, Domain.Entities.Support>(context), ISupportRepository
{
    private readonly DbSet<Domain.Entities.Support> _supports = context.Set<Domain.Entities.Support>();
    
    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _supports
            .AsNoTracking()
            .Where(support => support.AccountId == accountId)
            .Select(support => support.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsExistAsync(string firstName, string lastName, string middleName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _supports
            .AsNoTracking()
            .Where(support => support.FullName.FirstName == firstName
            && support.FullName.LastName == lastName
            && support.FullName.MiddleName == middleName)
            .AnyAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _supports
            .AsNoTracking()
            .CountAsync(ct);
    }

    public async Task<IReadOnlyCollection<SupportForAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return await _supports
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(support => new SupportForAdminProjection(
                support.Id,
                support.FullName
            ))
            .ToListAsync(ct);
    }
}