using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Media.Infrastructure.Repositories;

public sealed class BlobResourceRepository(MediaContext context)
    : BaseRepository<MediaContext, BlobResource>(context), IBlobResourceRepository
{
    private readonly DbSet<BlobResource> _blobResources = context.BlobResources;
    
    public async Task<IReadOnlyCollection<BlobResource>> GetAllForDeleteAsync(IReadOnlyCollection<Guid> eventMediaIds, CancellationToken ct)
        => await _blobResources
            .AsNoTracking()
            .Where(blob => eventMediaIds.Contains(blob.Id))
            .ToListAsync(ct);

    public async Task<BlobResource?> GetByUrlAsync(string mediaUrl, CancellationToken ct)
        => await _blobResources
            .Where(blob => blob.Url == mediaUrl)
            .FirstOrDefaultAsync(ct);
}