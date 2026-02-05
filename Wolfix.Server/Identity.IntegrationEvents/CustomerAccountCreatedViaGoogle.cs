using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record CustomerAccountCreatedViaGoogle : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
    
    public required string LastName { get; init; }
    
    public required string FirstName { get; init; }
    
    public required string PhotoUrl { get; init; }
}