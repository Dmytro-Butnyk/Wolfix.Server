using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Interfaces;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IIntegrationEvent
{
    Task<VoidResult> HandleAsync(TEvent @event, CancellationToken ct);
}

public interface IIntegrationEventHandler<in TEvent, TResult> where TEvent : IIntegrationEvent
{
    Task<Result<TResult>> HandleAsync(TEvent @event, CancellationToken ct);
}