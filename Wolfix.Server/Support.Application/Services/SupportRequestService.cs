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
    private const string SupportRequestsCollectionName = "support_requests";
    
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

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);
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

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);
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

        Result<BaseSupportRequest> createResult = await CreateSupportRequestByCategory(request, ct);
        
        if (createResult.IsFailure)
        {
            return VoidResult.Failure(createResult);
        }

        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);
        await supportRequests.InsertOneAsync(createResult.Value!, cancellationToken: ct);
        return VoidResult.Success();
    }
    
    private async Task<Result<BaseSupportRequest>> CreateSupportRequestByCategory(CreateSupportRequestDto request, CancellationToken ct)
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
            CreateOrderIssueSupportRequestDto orderIssueDto => Result<BaseSupportRequest>.Copy(
                await CreateOrderIssueSupportRequest(orderIssueDto, ct)
            ),
            _ => Result<BaseSupportRequest>.Failure("Invalid or unknown support category")
        };

    private async Task<Result<OrderIssueSupportRequest>> CreateOrderIssueSupportRequest(CreateOrderIssueSupportRequestDto request, CancellationToken ct)
    {
        var @event = new CheckCustomerOrder(request.OrderId, request.OrderNumber, request.CustomerId);
        VoidResult checkOrderData = await eventBus.PublishWithoutResultAsync(@event, ct);

        if (checkOrderData.IsFailure)
        {
            return Result<OrderIssueSupportRequest>.Failure(checkOrderData);
        }

        return OrderIssueSupportRequest.Create(
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.PhoneNumber,
            request.BirthDate,
            request.CustomerId,
            request.Category,
            request.Content,
            request.ExtraElements,
            request.OrderId,
            request.OrderNumber
        );
    }
    
    public async Task<Result<IReadOnlyCollection<SupportRequestShortDto>>> GetAllPendingAsync(string? category, CancellationToken ct)
    {
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);

        if (category is not null)
        {
            return await GetAllByCategoryAsync(category, ct);
        }
        
        IReadOnlyCollection<BaseSupportRequest> foundSupportRequests = await supportRequests
            .Find(sr => sr.Status == SupportRequestStatus.Pending)
            .SortByDescending(sr => sr.CreatedAt)
            .ToListAsync(ct);
        
        IReadOnlyCollection<SupportRequestShortDto> dto = foundSupportRequests
            .Select(fsr => new SupportRequestShortDto(
                fsr.Id,
                fsr.Category.ToString(),
                fsr.RequestContent,
                fsr.CreatedAt,
                fsr.GetAdditionalProperties()
            ))
            .ToList();
        
        return Result<IReadOnlyCollection<SupportRequestShortDto>>.Success(dto);
    }

    private async Task<Result<IReadOnlyCollection<SupportRequestShortDto>>> GetAllByCategoryAsync(string category,
        CancellationToken ct)
    {
        if (!Enum.TryParse<SupportRequestCategory>(category,true, out var categoryE))
        {
            return Result<IReadOnlyCollection<SupportRequestShortDto>>.Failure(
                $"Category: '{category}' is invalid.");
        }
        
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);
        
        IReadOnlyCollection<BaseSupportRequest> foundSupportRequests = await supportRequests
            .Find(sr => sr.Status == SupportRequestStatus.Pending && sr.Category == categoryE)
            .SortByDescending(sr => sr.CreatedAt)
            .ToListAsync(ct);

        IReadOnlyCollection<SupportRequestShortDto> dto = foundSupportRequests
            .Select(fsr => new SupportRequestShortDto(
                fsr.Id,
                fsr.Category.ToString(),
                fsr.RequestContent,
                fsr.CreatedAt,
                fsr.GetAdditionalProperties()
            ))
            .ToList();
        
        return Result<IReadOnlyCollection<SupportRequestShortDto>>.Success(dto);
    }

    public async Task<Result<IReadOnlyCollection<SupportRequestForCustomerShortDto>>> GetAllForCustomerAsync(
        Guid customerId, CancellationToken ct, string? category)
    {
        var @event = new CheckCustomerExistsForCreatingSupportRequest(customerId);
        VoidResult checkCustomerExistsResult = await eventBus.PublishWithoutResultAsync(@event, ct);

        if (checkCustomerExistsResult.IsFailure)
        {
            return Result<IReadOnlyCollection<SupportRequestForCustomerShortDto>>.Failure(checkCustomerExistsResult);
        }
        
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);

        IReadOnlyCollection<SupportRequestForCustomerShortProjection> projection;
        if (category is not null)
        {
            if (!Enum.TryParse<SupportRequestCategory>(category, out var categoryE))
            {
                return Result<IReadOnlyCollection<SupportRequestForCustomerShortDto>>.Failure(
                    $"Category '{category}' is invalid.");
            }
            
            projection = await supportRequests
                .Find(sr => sr.CustomerId == customerId && sr.Category == categoryE)
                .SortByDescending(sr => sr.CreatedAt)
                .Project(sr => new SupportRequestForCustomerShortProjection(
                    sr.Id,
                    sr.Category,
                    sr.RequestContent,
                    sr.Status,
                    sr.CreatedAt
                ))
                .ToListAsync(ct);
        }
        else
        {
            projection = await supportRequests
                .Find(sr => sr.CustomerId == customerId)
                .SortByDescending(sr => sr.CreatedAt)
                .Project(sr => new SupportRequestForCustomerShortProjection(
                    sr.Id,
                    sr.Category,
                    sr.RequestContent,
                    sr.Status,
                    sr.CreatedAt
                ))
                .ToListAsync(ct);
        }

        IReadOnlyCollection<SupportRequestForCustomerShortDto> dto = projection
            .Select(pr => pr.ToCustomerShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<SupportRequestForCustomerShortDto>>.Success(dto);
    }

    public async Task<Result<SupportRequestForCustomerDto>> GetForCustomerAsync(Guid customerId, Guid supportRequestId, CancellationToken ct)
    {
        var @event = new CheckCustomerExistsForCreatingSupportRequest(customerId);
        VoidResult checkCustomerExistsResult = await eventBus.PublishWithoutResultAsync(@event, ct);

        if (checkCustomerExistsResult.IsFailure)
        {
            return Result<SupportRequestForCustomerDto>.Failure(checkCustomerExistsResult);
        }
        
        IMongoCollection<BaseSupportRequest> supportRequests = mongoDb.GetCollection<BaseSupportRequest>(SupportRequestsCollectionName);

        BaseSupportRequest? supportRequest = await supportRequests
            .Find(sr => sr.Id == supportRequestId && sr.CustomerId == customerId)
            .FirstOrDefaultAsync(ct);

        if (supportRequest is null)
        {
            return Result<SupportRequestForCustomerDto>.Failure(
                $"Support request with id: {supportRequestId} not found",
                HttpStatusCode.NotFound
            );
        }

        SupportRequestForCustomerProjection projection = new(
            supportRequest.Id,
            supportRequest.Category,
            supportRequest.RequestContent,
            supportRequest.Status,
            supportRequest.ResponseContent,
            supportRequest.CreatedAt,
            supportRequest.ProcessedAt,
            supportRequest.GetAdditionalProperties()
        );

        SupportRequestForCustomerDto dto = projection.ToCustomerDto();
        
        return Result<SupportRequestForCustomerDto>.Success(dto);
    }
}