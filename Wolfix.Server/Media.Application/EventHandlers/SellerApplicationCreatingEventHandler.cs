using Media.Application.Dto;
using Media.Application.Interfaces;
using Media.Domain.Interfaces;
using Media.IntegrationEvents.Dto;
using Seller.IntegrationEvents;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class SellerApplicationCreatingEventHandler(IBlobResourceService blobResourceService)
    : IIntegrationEventHandler<SellerApplicationCreating, CreatedBlobResourceDto>
{
    public async Task<Result<CreatedBlobResourceDto>> HandleAsync(SellerApplicationCreating @event, CancellationToken ct)
    {
        Result<BlobResourceShortDto> createBlobResourceResult = await blobResourceService.AddBlobResourceAsync(
            BlobResourceType.Document,
            @event.Document.OpenReadStream(),
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