using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Entities;
using Support.Domain.Enums;
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
            .Where(sr => sr.Status == SupportRequestStatus.Pending)
            .Select(sr => new SupportRequestShortProjection(sr.Id, sr.Category.ToString(), sr.RequestContent, sr.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<SupportRequestShortProjection>> GetAllByCategoryAsync(
        SupportRequestCategory category, CancellationToken ct)
    {
        return await _supportRequests
            .AsNoTracking()
            .Include(sr => sr.ProcessedBy)
            .Where(sr => sr.Category == category && sr.Status == SupportRequestStatus.Pending)
            .Select(sr => new SupportRequestShortProjection(sr.Id, sr.Category.ToString(), sr.RequestContent, sr.CreatedAt))
            .ToListAsync(ct);
    }
}