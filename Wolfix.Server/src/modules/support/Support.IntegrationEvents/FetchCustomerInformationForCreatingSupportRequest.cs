using Shared.IntegrationEvents.Interfaces;

namespace Support.IntegrationEvents;

public sealed record FetchCustomerInformationForCreatingSupportRequest(Guid CustomerId): IIntegrationEvent;