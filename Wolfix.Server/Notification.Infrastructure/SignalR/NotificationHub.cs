using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Notification.Domain.Entities;
using Notification.Infrastructure.Interfaces;
using Notification.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Notification.Infrastructure.SignalR;

public sealed class NotificationHub(IEventBus eventBus, IConnectionMapping connectionMapping) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        var connectionId = Context.ConnectionId;
    
        // 1. Достаем данные из токена
        var accountId = Context.UserIdentifier; // Это NameIdentifier
        var profileIdString = user?.Claims.FirstOrDefault(c => c.Type == "profile_id")?.Value;
        var role = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (accountId == null || profileIdString == null || role == null)
        {
            // Если токен кривой — рвем соединение
            Context.Abort();
            return;
        }

        var profileId = Guid.Parse(profileIdString);

        // 2. Сохраняем в наш маппинг
        var info = new ConnectionInfo(accountId, profileId, role, connectionId);
        connectionMapping.Add(connectionId, info);

        // 3. Подписываем на личную группу ПРОФИЛЯ (удобно слать личные уведы)
        await Groups.AddToGroupAsync(connectionId, $"profile:{profileId}");
        await Groups.AddToGroupAsync(connectionId, $"SYSTEM_NOTIFICATIONS");
        

        // 4. Логика для Customer
        if (role == "Customer")
        {
            // Публикуем событие, чтобы подтянуть избранное.
            // ВАЖНО: Передай ConnectionId в событии, чтобы хендлер знал, кого подписывать!
            await eventBus.PublishWithoutResultAsync(
                new FetchUserFavorites(profileId, connectionId), // Добавь ConnectionId в ивент
                Context.ConnectionAborted
            );
        }
        // 5. Логика для Seller (на будущее)
        else if (role == "Seller")
        {
            // await Groups.AddToGroupAsync(connectionId, "sellers_global"); 
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        connectionMapping.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}