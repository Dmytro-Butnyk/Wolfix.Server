using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Shared.IntegrationEvents;

public sealed class EventBus(IServiceScopeFactory serviceProvider) : IEventBus
{
    public async Task<VoidResult> PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent
    {
        using var scope = serviceProvider.CreateScope();
        
        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>()
            .ToList();

        foreach (var handler in handlers)
        {
            VoidResult result = await handler.HandleAsync(@event, ct);

            if (!result.IsSuccess)
            {
                return VoidResult.Failure(result.ErrorMessage!, result.StatusCode);
            }
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> PublishForParallelAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent
    {
        //todo
        using var scope = serviceProvider.CreateScope();
        
        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>()
            .ToList();

        var tasks = handlers.Select(handler => handler.HandleAsync(@event, ct));
        
        var results = await Task.WhenAll(tasks);
        
        if (results.Any(result => !result.IsSuccess))
        {
            VoidResult firstUnsuccessResult = results.First(result => !result.IsSuccess);
            return VoidResult.Failure(firstUnsuccessResult.ErrorMessage!, firstUnsuccessResult.StatusCode);
        }
        
        return VoidResult.Success();
    }
}