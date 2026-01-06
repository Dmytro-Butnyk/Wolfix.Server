using System.Net;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Support.Application.Dto;
using Support.Application.Dto.SupportRequest;
using Support.Application.Dto.SupportRequest.Create;
using Support.Application.Mapping;
using Support.Domain.Entities;
using Support.Domain.Entities.SupportRequests;
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

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>("support_requests");
        BaseSupportRequest? supportRequest = await supportRequests.Find(sr => sr.Id == supportRequestId).FirstOrDefaultAsync(ct);

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
        
        var filter = Builders<BaseSupportRequest>.Filter.Eq(sr => sr.Id, supportRequestId);
        var update = Builders<BaseSupportRequest>.Update
            .Set(sr => sr.Status, supportRequest.Status)
            .Set(sr => sr.SupportId, supportRequest.SupportId)
            .Set(sr => sr.ResponseContent, supportRequest.ResponseContent)
            .Set(sr => sr.ProcessedAt, supportRequest.ProcessedAt);
        
        await supportRequests.UpdateOneAsync(filter, update, cancellationToken: ct);
        
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

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>("support_requests");
        BaseSupportRequest? supportRequest = await supportRequests.Find(sr => sr.Id == supportRequestId).FirstOrDefaultAsync(ct);

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
        
        var filter = Builders<BaseSupportRequest>.Filter.Eq(sr => sr.Id, supportRequestId);
        var update = Builders<BaseSupportRequest>.Update
            .Set(sr => sr.Status, supportRequest.Status)
            .Set(sr => sr.SupportId, supportRequest.SupportId)
            .Set(sr => sr.ProcessedAt, supportRequest.ProcessedAt);
        
        await supportRequests.UpdateOneAsync(filter, update, cancellationToken: ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> CreateAsync(CreateSupportRequestDto request, CancellationToken ct)
    {
        VoidResult checkCustomerExistsResult = await eventBus.PublishWithoutResultAsync(new CheckCustomerExistsForCreatingSupportRequest(request.CustomerId), ct);
        
        if (checkCustomerExistsResult.IsFailure)
        {
            return checkCustomerExistsResult;
        }

        Result<CustomerInformationForSupportRequestDto> fetchCustomerInfoResult = await eventBus
            .PublishWithSingleResultAsync<FetchCustomerInformationForCreatingSupportRequest, CustomerInformationForSupportRequestDto>(
                new FetchCustomerInformationForCreatingSupportRequest(request.CustomerId),
                ct);
        
        if (fetchCustomerInfoResult.IsFailure)
        {
            return VoidResult.Failure(fetchCustomerInfoResult);
        }

        Result<BaseSupportRequest> createResult = GetSupportRequestByCategory(request);
        
        if (createResult.IsFailure)
        {
            return VoidResult.Failure(createResult);
        }

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>("support_requests");
        await supportRequests.InsertOneAsync(createResult.Value!, cancellationToken: ct);
        return VoidResult.Success();
    }
    
    private Result<BaseSupportRequest> GetSupportRequestByCategory(CreateSupportRequestDto request)
        => request switch
        {
            CreateBugOrErrorSupportRequestDto orderDto => Result<BaseSupportRequest>.Copy(BugOrErrorSupportRequest.Create(
                orderDto.FirstName,
                orderDto.LastName,
                orderDto.MiddleName,
                orderDto.PhoneNumber,
                orderDto.BirthDate,
                orderDto.CustomerId,
                orderDto.Category,
                orderDto.Content,
                orderDto.ExtraElements,
                orderDto.PhotoUrl
            )),
            CreateGeneralSupportRequestDto generalDto => Result<BaseSupportRequest>.Copy(GeneralSupportRequest.Create(
                generalDto.FirstName,
                generalDto.LastName,
                generalDto.MiddleName,
                generalDto.PhoneNumber,
                generalDto.BirthDate,
                generalDto.CustomerId,
                generalDto.Category,
                generalDto.Content,
                generalDto.ExtraElements
            )),
            CreateOrderIssueSupportRequestDto orderIssueDto => Result<BaseSupportRequest>.Copy(OrderIssueSupportRequest.Create(
                orderIssueDto.FirstName,
                orderIssueDto.LastName,
                orderIssueDto.MiddleName,
                orderIssueDto.PhoneNumber,
                orderIssueDto.BirthDate,
                orderIssueDto.CustomerId,
                orderIssueDto.Category,
                orderIssueDto.Content,
                orderIssueDto.ExtraElements,
                orderIssueDto.OrderId
            )),
            _ => Result<BaseSupportRequest>.Failure("Invalid or unknown support category")
        };
    
    public async Task<IReadOnlyCollection<SupportRequestShortDto>> GetAllPendingAsync(CancellationToken ct)
    {
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>("support_requests");
        
        IReadOnlyCollection<SupportRequestShortProjection> projection = await supportRequests
            .Find(sr => sr.Status == SupportRequestStatus.Pending)
            .SortByDescending(sr => sr.CreatedAt)
            .Project(sr => new SupportRequestShortProjection(sr.Id, sr.Category.ToString(), sr.RequestContent, sr.CreatedAt))
            .ToListAsync(ct);

        IReadOnlyCollection<SupportRequestShortDto> dto = projection
            .Select(pr => pr.ToShortDto())
            .ToList();
        
        return dto;
    }

    public async Task<Result<IReadOnlyCollection<SupportRequestShortDto>>> GetAllByCategoryAsync(string category,
        CancellationToken ct)
    {
        if (!Enum.TryParse<SupportRequestCategory>(category,true, out var categoryE))
        {
            return Result<IReadOnlyCollection<SupportRequestShortDto>>.Failure(
                $"Category '{category}' is invalid.");
        }
        
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>("support_requests");
        
        IReadOnlyCollection<SupportRequestShortProjection> projection = await supportRequests
            .Find(sr => sr.Status == SupportRequestStatus.Pending && sr.Category == categoryE)
            .SortByDescending(sr => sr.CreatedAt)
            .Project(sr => new SupportRequestShortProjection(sr.Id, sr.Category.ToString(), sr.RequestContent, sr.CreatedAt))
            .ToListAsync(ct);
        
        IReadOnlyCollection<SupportRequestShortDto> dto = projection
            .Select(pr => pr.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<SupportRequestShortDto>>.Success(dto);
    }
}