using Media.Application.Dto;
using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Media.Application.Interfaces;

public interface IBlobResourceService
{
    Task<Result<BlobResourceShortDto>> AddBlobResourceAsync(
        BlobResourceType contentType,
        IFormFile fileData,
        CancellationToken ct);
    
    Task<VoidResult> DeleteBlobResourceAsync(Guid id, CancellationToken ct);
    
    Task ExecuteDeleteBlobResourceAsync(IReadOnlyCollection<Guid> mediaIds, CancellationToken ct);
}