using System.Net;
using Catalog.IntegrationEvents;
using Customer.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

public sealed class CheckCustomerExistsForAddingReviewEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CheckCustomerExistsForAddingReview>
{
    public async Task<VoidResult> HandleAsync(CheckCustomerExistsForAddingReview @event, CancellationToken ct)
    {
        if (!await customerRepository.IsExistAsync(@event.CustomerId, ct))
        {
            return VoidResult.Failure(
                $"Customer with id: {@event.CustomerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return VoidResult.Success();
    }
}