using Media.Application.Dto;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Media.Application.Interfaces;

public interface IBlobResourceService
{
    Task<Result<BlobResourceShortDto>> AddBlobResourceAsync(
        BlobResourceType contentType,
        Stream fileStream,
        CancellationToken ct);
    
    Task<VoidResult> DeleteBlobResourceAsync(Guid id, CancellationToken ct);
}