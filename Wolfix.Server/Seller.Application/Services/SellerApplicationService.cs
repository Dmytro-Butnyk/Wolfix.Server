using System.Net;
using Media.IntegrationEvents.Dto;
using Seller.Application.Dto.SellerApplication;
using Seller.Application.Mapping.SellerApplication;
using Seller.Domain.Interfaces;
using Seller.Domain.Projections.SellerApplication;
using Seller.Domain.SellerApplicationAggregate;
using Seller.Domain.SellerApplicationAggregate.Enums;
using Seller.Domain.Services;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Seller.Application.Services;

public sealed class SellerApplicationService(
    ISellerApplicationRepository sellerApplicationRepository,
    EventBus eventBus,
    SellerDomainService sellerDomainService)
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

        VoidResult checkCategoryExistResult = await eventBus.PublishWithoutResultAsync(new CheckCategoryExist
        {
            CategoryId = request.CategoryId,
            Name = request.CategoryName
        }, ct);

        if (checkCategoryExistResult.IsFailure)
        {
            return checkCategoryExistResult;
        }
        
        IReadOnlyCollection<SellerApplication> existingSellerApplications =
            await sellerApplicationRepository.GetByAccountIdAsNoTrackingAsync(accountId, ct);
        
        bool customerIsAlreadySeller =
            existingSellerApplications.Count != 0 && existingSellerApplications.Any(sa => sa.Status == SellerApplicationStatus.Approved);

        if (customerIsAlreadySeller)
        {
            return VoidResult.Failure("Customer is already a seller");
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
        
        Result<SellerApplication> createSellerApplicationResult = SellerApplication.Create(accountId, request.CategoryId,
            request.CategoryName, blobResourceId, documentUrl, request.FirstName, request.LastName, request.MiddleName,
            request.PhoneNumber, request.City, request.Street, request.HouseNumber, request.ApartmentNumber, request.BirthDate);

        if (createSellerApplicationResult.IsFailure)
        {
            return VoidResult.Failure(createSellerApplicationResult);
        }

        SellerApplication sellerApplication = createSellerApplicationResult.Value!;

        await sellerApplicationRepository.AddAsync(sellerApplication, ct);
        await sellerApplicationRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<IReadOnlyCollection<SellerApplicationDto>> GetPendingApplicationsAsync(CancellationToken ct)
    {
        IReadOnlyCollection<SellerApplicationProjection> pendingApplications = 
            await sellerApplicationRepository.GetPendingApplicationsAsync(ct);

        return pendingApplications
            .Select(pa => pa.ToDto())
            .ToList();
    }

    public async Task<VoidResult> ApproveApplicationAsync(Guid sellerApplicationId, CancellationToken ct)
    {
        SellerApplication? application = await sellerApplicationRepository.GetByIdAsync(sellerApplicationId, ct);

        if (application is null)
        {
            return VoidResult.Failure(
                $"Seller application with id: {sellerApplicationId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult approveApplicationResult = application.Approve();

        if (approveApplicationResult.IsFailure)
        {
            return approveApplicationResult;
        }

        VoidResult addSellerRoleToAccountResult = await eventBus.PublishWithoutResultAsync(new SellerApplicationApproved
        {
            AccountId = application.AccountId
        }, ct);

        if (addSellerRoleToAccountResult.IsFailure)
        {
            return addSellerRoleToAccountResult;
        }
        
        await sellerApplicationRepository.SaveChangesAsync(ct);

        VoidResult createSellerResult = await sellerDomainService.CreateSellerWithFirstCategoryAsync(
            application.AccountId,
            application.SellerProfileData,
            application.CategoryId,
            application.CategoryName,
            ct
        );

        if (createSellerResult.IsFailure)
        {
            return createSellerResult;
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> RejectApplicationAsync(Guid sellerApplicationId, CancellationToken ct)
    {
        SellerApplication? application = await sellerApplicationRepository.GetByIdAsync(sellerApplicationId, ct);

        if (application is null)
        {
            return VoidResult.Failure(
                $"Seller application with id: {sellerApplicationId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult rejectApplicationResult = application.Reject();

        if (rejectApplicationResult.IsFailure)
        {
            return rejectApplicationResult;
        }
        
        await sellerApplicationRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}