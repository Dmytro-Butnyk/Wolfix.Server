using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Interfaces;

public interface IEventBus
{
    Task<VoidResult> PublishWithoutResultAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
    
    Task<Result<TResult>> PublishWithSingleResultAsync<TEvent, TResult>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
}