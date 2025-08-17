namespace Shared.IntegrationEvents.Inerfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
    
    Task PublishForParallelAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent;
}