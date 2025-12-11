using System.Net;
using Shared.Domain.Models;
using Support.Application.Dto;
using Support.Domain.Entities;
using Support.Domain.Interfaces;

namespace Support.Application.Services;

public sealed class SupportRequestService(ISupportRequestRepository supportRequestRepository, ISupportRepository supportRepository)
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
}