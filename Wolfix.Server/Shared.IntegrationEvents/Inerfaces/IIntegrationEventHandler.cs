using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Inerfaces;

public interface IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    Task<VoidResult> HandleAsync(TEvent @event, CancellationToken ct);
}