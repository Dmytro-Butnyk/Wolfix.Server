using Customer.IntegrationEvents;
using Microsoft.AspNetCore.SignalR;
using Notification.Infrastructure.Interfaces;
using Notification.Infrastructure.SignalR;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Notification.Application.EventHandlers;

public sealed class FetchedUserFavoritesEventHandler(
    IHubContext<NotificationHub> hubContext,
    IConnectionMapping connectionMapping)
    : IIntegrationEventHandler<FetchedUserFavorites>
{
    public async Task<VoidResult> HandleAsync(FetchedUserFavorites @event, CancellationToken ct)
    {
        // @event.ConnectionId - тот самый ID, который мы передали из Хаба
        var connectionId = @event.ConnectionId;

        // Проверяем, жив ли еще коннект (опционально, через маппинг)
        if (connectionMapping.GetInfo(connectionId) == null)
        {
            return VoidResult.Failure("The client is not connected!");
        }

        foreach (var productId in @event.FavoriteItems)
        {
            // Подписываем это конкретное соединение на изменения товара
            await hubContext.Groups.AddToGroupAsync(connectionId, $"product:{productId}", ct);

            await hubContext.Clients.Group($"product:{productId}")
                .SendAsync("ReceiveMessage", $"Продукт в избранном: {productId}", cancellationToken: ct);
        }
        
        // Todo: remove timely test notification
        // --- Временная проверка ---
        await hubContext.Clients.Client(connectionId)
            .SendAsync("ReceiveMessage", "Тестовое уведомление для продукта", cancellationToken: ct);
        
        await hubContext.Clients.Group("SYSTEM_NOTIFICATIONS")
            .SendAsync("ReceiveMessage", "Тестовое системное уведомление", cancellationToken: ct);
        // --- Конец временной проверки ---

        return VoidResult.Success();
    }
}