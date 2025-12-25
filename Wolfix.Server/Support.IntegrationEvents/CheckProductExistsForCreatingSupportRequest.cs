using Shared.IntegrationEvents.Interfaces;

namespace Support.IntegrationEvents;

// Unused event
public sealed record CheckProductExistsForCreatingSupportRequest(Guid ProductId) : IIntegrationEvent;