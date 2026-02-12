using Microsoft.AspNetCore.Http;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.IntegrationEvents;

public sealed record SellerApplicationCreating : IIntegrationEvent
{
    public required IFormFile Document { get; init; }
}