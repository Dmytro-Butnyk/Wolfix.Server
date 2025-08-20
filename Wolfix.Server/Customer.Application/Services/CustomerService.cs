using System.Net;
using Customer.Application.Dto;
using Customer.Application.Interfaces;
using Customer.Domain.Interfaces;
using Customer.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.Services;

internal sealed class CustomerService(ICustomerRepository customerRepository, IEventBus eventBus) : ICustomerService
{
    public async Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsNoTrackingAsync(request.CustomerId, ct);

        if (customer is null)
        {
            return VoidResult.Failure("Customer not found", HttpStatusCode.NotFound);
        }

        VoidResult result = await eventBus.PublishAsync(new CheckProductExists
        {
            ProductId = request.ProductId,
            CustomerId = request.CustomerId
        }, ct);

        return result;
    }
}