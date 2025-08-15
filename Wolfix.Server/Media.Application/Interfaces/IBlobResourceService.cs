using Shared.Domain.Models;

namespace Media.Application.Interfaces;

public interface IBlobResourceService
{
    Task<VoidResult> AddBlobResourceAsync(
        string contentType,
        Stream fileStream,
        CancellationToken ct);
    
    Task<VoidResult> DeleteBlobResourceAsync(Guid id, CancellationToken ct);
}