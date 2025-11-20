using Microsoft.AspNetCore.Http;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record SellerAccountCreated : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
    
    public required string Email { get; init; }
    
    public required string Password { get; init; }
    
    public required string FirstName { get; init; }
    
    public required string LastName { get; init; }
    
    public required string MiddleName { get; init; }
    
    public required string PhoneNumber { get; init; }
    
    public required string City { get; init; }
    
    public required string Street { get; init; }
    
    public required uint HouseNumber { get; init; }
    
    public required uint? ApartmentNumber { get; init; }
    
    public required DateOnly BirthDate { get; init; }
    
    public required IFormFile Document { get; init; }
}