using Microsoft.AspNetCore.Http;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record SellerAccountCreated : IIntegrationEvent
{
    public Guid AccountId { get; init; }
    
    public string Email { get; init; }
    
    public string Password { get; init; }
    
    public string FirstName { get; init; }
    
    public string LastName { get; init; }
    
    public string MiddleName { get; init; }
    
    public string PhoneNumber { get; init; }
    
    public string City { get; init; }
    
    public string Street { get; init; }
    
    public uint HouseNumber { get; init; }
    
    public uint? ApartmentNumber { get; init; }
    
    public DateOnly BirthDate { get; init; }
    
    public IFormFile Document { get; init; }
}