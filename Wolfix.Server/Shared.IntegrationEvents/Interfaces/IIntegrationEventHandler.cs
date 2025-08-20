using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Interfaces;

public interface IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    Task<VoidResult> HandleAsync(TEvent @event, CancellationToken ct);
}