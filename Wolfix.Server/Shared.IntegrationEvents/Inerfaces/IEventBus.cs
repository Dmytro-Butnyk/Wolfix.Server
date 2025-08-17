using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Inerfaces;

public interface IEventBus
{
    Task<VoidResult> PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
    
    Task<VoidResult> PublishForParallelAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
}