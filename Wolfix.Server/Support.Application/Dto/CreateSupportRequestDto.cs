using System.Text.Json.Serialization;

namespace Support.Application.Dto;

public sealed record CreateSupportRequestDto(
    Guid CustomerId,
    string Category,
    string Content)
{
    [JsonExtensionData]
    public Dictionary<string, object> ExtraElements { get; init; } = [];
}