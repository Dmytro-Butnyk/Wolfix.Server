using System.Net;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.Application.Dto;
using Support.Application.Mapping;
using Support.Domain.Entities;
using Support.Domain.Interfaces;
using Support.Domain.Projections;
using Support.IntegrationEvents;

namespace Support.Application.Services;

public sealed class SupportRequestService(
    ISupportRequestRepository supportRequestRepository,
    ISupportRepository supportRepository,
    IEventBus eventBus)
{
    public async Task<VoidResult> RespondAsync(Guid supportId, Guid supportRequestId, RespondOnRequestDto request, CancellationToken ct)
    {
        Domain.Entities.Support? support = await supportRepository.GetByIdAsNoTrackingAsync(supportId, ct);

        if (support is null)
        {
            return VoidResult.Failure(
                $"Support with id: {supportId} not found",
                HttpStatusCode.NotFound
            );
        }

        SupportRequest? supportRequest = await supportRequestRepository.GetByIdAsync(supportRequestId, ct);

        if (supportRequest is null)
        {
            return VoidResult.Failure(
                $"Support request with id: {supportRequestId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult respondOnRequestResult = supportRequest.Respond(support, request.Content);

        if (respondOnRequestResult.IsFailure)
        {
            return respondOnRequestResult;
        }
        
        await supportRequestRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> CancelAsync(Guid supportId, Guid supportRequestId, CancellationToken ct)
    {
        Domain.Entities.Support? support = await supportRepository.GetByIdAsNoTrackingAsync(supportId, ct);

        if (support is null)
        {
            return VoidResult.Failure(
                $"Support with id: {supportId} not found",
                HttpStatusCode.NotFound
            );
        }

        SupportRequest? supportRequest = await supportRequestRepository.GetByIdAsync(supportRequestId, ct);

        if (supportRequest is null)
        {
            return VoidResult.Failure(
                $"Support request with id: {supportRequestId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult cancelRequestAsync = supportRequest.Cancel(support);

        if (cancelRequestAsync.IsFailure)
        {
            return cancelRequestAsync;
        }
        
        await supportRequestRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> CreateAsync(CreateSupportRequestDto request, CancellationToken ct)
    {
        VoidResult checkCustomerExistsResult = await eventBus.PublishWithoutResultAsync(new CheckCustomerExistsForCreatingSupportRequest(request.CustomerId), ct);

        if (checkCustomerExistsResult.IsFailure)
        {
            return checkCustomerExistsResult;
        }

        if (request.ProductId is not null)
        {
            VoidResult checkProductExistsResult = await eventBus.PublishWithoutResultAsync(new CheckProductExistsForCreatingSupportRequest(request.ProductId.Value), ct);
            
            if (checkProductExistsResult.IsFailure)
            {
                return checkProductExistsResult;
            }
        }

        Result<SupportRequest> createSupportRequestResult = SupportRequest.Create(request.Email, request.FirstName,
            request.LastName, request.MiddleName, request.PhoneNumber, request.BirthDate, request.CustomerId,
            request.Title, request.Content, request.ProductId);

        if (createSupportRequestResult.IsFailure)
        {
            return VoidResult.Failure(createSupportRequestResult);
        }

        await supportRequestRepository.AddAsync(createSupportRequestResult.Value!, ct);
        await supportRequestRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<IReadOnlyCollection<SupportRequestShortDto>> GetAllPendingAsync(CancellationToken ct)
    {
        IReadOnlyCollection<SupportRequestShortProjection> projection = await supportRequestRepository.GetAllPendingAsync(ct);

        IReadOnlyCollection<SupportRequestShortDto> dto = projection
            .Select(pr => pr.ToShortDto())
            .ToList();
        
        return dto;
    }
}