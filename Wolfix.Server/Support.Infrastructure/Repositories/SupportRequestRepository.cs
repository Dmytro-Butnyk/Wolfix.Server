using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Entities;
using Support.Domain.Interfaces;
using Support.Domain.Projections;

namespace Support.Infrastructure.Repositories;

internal sealed class SupportRequestRepository(SupportContext context)
    : BaseRepository<SupportContext, SupportRequest>(context), ISupportRequestRepository
{
    private readonly DbSet<SupportRequest> _supportRequests = context.Set<SupportRequest>();
    
    public async Task<IReadOnlyCollection<SupportRequestShortProjection>> GetAllPendingAsync(CancellationToken ct)
    {
        return await _supportRequests
            .AsNoTracking()
            .Include(sr => sr.ProcessedBy)
            .Where(sr => sr.IsProcessed == false)
            .Select(sr => new SupportRequestShortProjection(sr.Id, sr.FullName, sr.PhoneNumber, sr.Title, sr.CreatedAt))
            .ToListAsync(ct);
    }
}