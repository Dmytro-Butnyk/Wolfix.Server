using Shared.IntegrationEvents.Interfaces;

namespace Customer.IntegrationEvents;

public record FetchedUserFavorites(Guid UserId,
    IReadOnlyCollection<Guid> FavoriteItems, string ConnectionId) : IIntegrationEvent;