using Shared.Domain.Models;

namespace Shared.IntegrationEvents.Interfaces;

public interface IEventBus
{
    Task<VoidResult> PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
    
    Task<VoidResult> PublishForParallelAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
}