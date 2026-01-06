using System.Text.Json.Serialization;

namespace Support.Application.Dto.SupportRequest.Create;

// public sealed record CreateSupportRequestDto(
//     Guid CustomerId,
//     string Category,
//     string Content)
// {
//     [JsonExtensionData]
//     public Dictionary<string, object> ExtraElements { get; init; } = [];
// }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "category")]
[JsonDerivedType(typeof(CreateBugOrErrorSupportRequestDto), typeDiscriminator: "BugOrError")]
[JsonDerivedType(typeof(CreateGeneralSupportRequestDto), typeDiscriminator: "General")]
[JsonDerivedType(typeof(CreateOrderIssueSupportRequestDto), typeDiscriminator: "OrderIssue")]
public abstract record CreateSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Category,
    string Content
);