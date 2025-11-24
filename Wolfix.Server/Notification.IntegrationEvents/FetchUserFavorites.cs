using Shared.IntegrationEvents.Interfaces;

namespace Notification.IntegrationEvents;

public record FetchUserFavorites(Guid UserId, string ConnectionId) : IIntegrationEvent;