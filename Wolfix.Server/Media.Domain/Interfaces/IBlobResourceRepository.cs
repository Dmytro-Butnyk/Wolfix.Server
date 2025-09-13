using Media.Domain.BlobAggregate;
using Media.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Media.Domain.Interfaces;

public interface IBlobResourceRepository : IBaseRepository<BlobResource>
{
    Task<IReadOnlyCollection<BlobResource>> GetAllForDeleteAsync(IReadOnlyCollection<Guid> eventMediaIds, CancellationToken ct);
}