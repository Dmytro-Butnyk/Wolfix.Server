using Media.IntegrationEvents.Dto;
using Seller.Application.Dto.SellerApplication;
using Seller.Application.Interfaces;
using Seller.Domain.Interfaces;
using Seller.Domain.SellerApplicationAggregate;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.Services;

internal sealed class SellerApplicationService(
    ISellerApplicationRepository sellerApplicationRepository,
    IEventBus eventBus)
    : ISellerApplicationService
{
    public async Task<VoidResult> CreateAsync(Guid accountId, CreateSellerApplicationDto request, CancellationToken ct)
    {
        VoidResult checkAccountExistResult = await eventBus.PublishWithoutResultAsync(new CustomerWantsToBeSeller
        {
            AccountId = accountId
        }, ct);

        if (checkAccountExistResult.IsFailure)
        {
            return checkAccountExistResult;
        }

        var @event = new SellerApplicationCreating
        {
            Document = request.Document
        };
        
        Result<CreatedBlobResourceDto> createBlobResourceResult =
            await eventBus.PublishWithSingleResultAsync<SellerApplicationCreating, CreatedBlobResourceDto>(@event, ct);

        if (createBlobResourceResult.IsFailure)
        {
            return VoidResult.Failure(createBlobResourceResult);
        }

        (Guid blobResourceId, string documentUrl) = createBlobResourceResult.Value!;
        
        Result<SellerApplication> createSellerApplicationResult = SellerApplication.Create(accountId, request.CategoryName,
            blobResourceId, documentUrl, request.FirstName, request.LastName, request.MiddleName, request.PhoneNumber,
            request.City, request.Street, request.HouseNumber, request.ApartmentNumber, request.BirthDate);

        if (createSellerApplicationResult.IsFailure)
        {
            return VoidResult.Failure(createSellerApplicationResult);
        }

        SellerApplication sellerApplication = createSellerApplicationResult.Value!;

        await sellerApplicationRepository.AddAsync(sellerApplication, ct);
        await sellerApplicationRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}