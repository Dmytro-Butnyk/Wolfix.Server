using System.Net;
using Order.Domain.Interfaces.Order;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Order.Application.EventHandlers;

public sealed class CheckCustomerOrderEventHandler(IOrderRepository orderRepository)
    : IIntegrationEventHandler<CheckCustomerOrder>
{
    public async Task<VoidResult> HandleAsync(CheckCustomerOrder @event, CancellationToken ct)
    {
        Domain.OrderAggregate.Order? customerOrder = await orderRepository.GetCustomerOrderAsync(@event.OrderId, @event.CustomerId, ct);

        if (customerOrder is null)
        {
            return VoidResult.Failure(
                $"Order with id: {@event.OrderId} not found",
                HttpStatusCode.NotFound
            );
        }

        if (customerOrder.Number != @event.OrderNumber)
        {
            return VoidResult.Failure("Customer order number mismatch.");
        }

        return VoidResult.Success();
    }
}