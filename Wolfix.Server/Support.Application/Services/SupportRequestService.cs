using System.Net;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Support.Application.Dto;
using Support.Application.Mapping;
using Support.Domain.Entities;
using Support.Domain.Enums;
using Support.Domain.Interfaces;
using Support.Domain.Projections;
using Support.IntegrationEvents;
using Support.IntegrationEvents.Dto;

namespace Support.Application.Services;

public sealed class SupportRequestService(
    ISupportRepository supportRepository,
    IMongoDatabase mongoDb,
    EventBus eventBus)
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

        IMongoCollection<SupportRequest> supportRequests = mongoDb.GetCollection<SupportRequest>("supportRequests");
        SupportRequest? supportRequest = await supportRequests.Find(sr => sr.Id == supportRequestId).FirstOrDefaultAsync(ct);

        if (supportRequest is null)
        {
            return VoidResult.Failure(
                $"Support request with id: {supportRequestId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult respondOnRequestResult = supportRequest.Respond(support.Id, request.Content);

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

        VoidResult cancelRequestAsync = supportRequest.Cancel(support.Id);

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

        Result<CustomerInformationForSupportRequestDto> fetchCustomerInfoResult
            = await eventBus.PublishWithSingleResultAsync<
                FetchCustomerInformationForCreatingSupportRequest,
                CustomerInformationForSupportRequestDto>(
                new FetchCustomerInformationForCreatingSupportRequest(request.CustomerId),
                ct);

        if (fetchCustomerInfoResult.IsFailure)
        {
            return VoidResult.Failure(fetchCustomerInfoResult);
        }

        Result<SupportRequest> createSupportRequestResult = SupportRequest.Create(fetchCustomerInfoResult.Value!.FirstName,
            fetchCustomerInfoResult.Value!.LastName, fetchCustomerInfoResult.Value!.MiddleName,
            fetchCustomerInfoResult.Value!.PhoneNumber, fetchCustomerInfoResult.Value.BirthDate,
            request.CustomerId, request.Category, request.Content, request.ExtraElements);

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

    public async Task<Result<IReadOnlyCollection<SupportRequestShortDto>>> GetAllByCategoryAsync(string category,
        CancellationToken ct)
    {
        if(!Enum.TryParse<SupportRequestCategory>(category,true, out var categoryE))
        {
            return Result<IReadOnlyCollection<SupportRequestShortDto>>.Failure(
                $"Category '{category}' is invalid.");
        }
        
        IReadOnlyCollection<SupportRequestShortProjection> projection = await supportRequestRepository.GetAllByCategoryAsync(categoryE, ct);
        
        IReadOnlyCollection<SupportRequestShortDto> dto = projection
            .Select(pr => pr.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<SupportRequestShortDto>>.Success(dto);
    }
}