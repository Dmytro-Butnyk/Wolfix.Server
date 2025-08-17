using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents.Inerfaces;

namespace Shared.IntegrationEvents;

public sealed class EventBus(IServiceScopeFactory serviceProvider) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent
    {
        using var scope = serviceProvider.CreateScope();
        
        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>()
            .ToList();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, ct);
        }
    }

    public async Task PublishForParallelAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent
    {
        using var scope = serviceProvider.CreateScope();
        
        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>()
            .ToList();

        var tasks = handlers.Select(handler => handler.HandleAsync(@event, ct));
        await Task.WhenAll(tasks);
    }
}