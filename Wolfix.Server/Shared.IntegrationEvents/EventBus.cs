using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Shared.IntegrationEvents;

public sealed class EventBus(IServiceScopeFactory serviceProvider) : IEventBus
{
    public async Task<VoidResult> PublishWithoutResultAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IIntegrationEvent
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        
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

    public async Task<Result<TResult>> PublishWithSingleResultAsync<TEvent, TResult>(TEvent @event,
        CancellationToken ct) where TEvent : IIntegrationEvent
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<IIntegrationEventHandler<TEvent, TResult>>();

        return await handler.HandleAsync(@event, ct);
    }
}