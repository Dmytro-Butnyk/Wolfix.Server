using Media.Application.Dto;
using Media.Application.Services;
using Media.IntegrationEvents.Dto;
using Seller.IntegrationEvents;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class SellerApplicationCreatingEventHandler(BlobResourceService blobResourceService)
    : IIntegrationEventHandler<SellerApplicationCreating, CreatedBlobResourceDto>
{
    public async Task<Result<CreatedBlobResourceDto>> HandleAsync(SellerApplicationCreating @event, CancellationToken ct)
    {
        Result<BlobResourceShortDto> createBlobResourceResult = await blobResourceService.AddBlobResourceAsync(
            BlobResourceType.Document,
            @event.Document,
            ct
        );

        if (createBlobResourceResult.IsFailure)
        {
            return Result<CreatedBlobResourceDto>.Failure(createBlobResourceResult);
        }

        BlobResourceShortDto blobResource = createBlobResourceResult.Value!;

        return Result<CreatedBlobResourceDto>.Success(new(blobResource.Id, blobResource.Url));
    }
}