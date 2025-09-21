using Microsoft.AspNetCore.Http;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record AddPhotoForNewCategory : IIntegrationEvent
{
    public required IFormFile FileData { get; init; }
}