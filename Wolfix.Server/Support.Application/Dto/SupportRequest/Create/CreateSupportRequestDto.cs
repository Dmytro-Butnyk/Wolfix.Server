using System.Text.Json.Serialization;
using Support.Domain.Enums;

namespace Support.Application.Dto.SupportRequest.Create;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(CreateBugOrErrorSupportRequestDto), typeDiscriminator: nameof(SupportRequestCategory.BugOrError))]
[JsonDerivedType(typeof(CreateGeneralSupportRequestDto), typeDiscriminator: nameof(SupportRequestCategory.General))]
[JsonDerivedType(typeof(CreateOrderIssueSupportRequestDto), typeDiscriminator: nameof(SupportRequestCategory.OrderIssue))]
public abstract record CreateSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Category,
    string Content
)
{
    [JsonExtensionData]
    public Dictionary<string, object> ExtraElements { get; init; } = [];
}