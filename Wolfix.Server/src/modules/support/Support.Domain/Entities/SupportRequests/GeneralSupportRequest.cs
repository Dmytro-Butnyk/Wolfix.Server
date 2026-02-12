using Shared.Domain.Models;

namespace Support.Domain.Entities.SupportRequests;

public sealed class GeneralSupportRequest : BaseSupportRequest
{
    private GeneralSupportRequest() { }

    private GeneralSupportRequest(SupportRequestCreateData createData, Guid customerId,
        string requestContent, Dictionary<string, object> extraElements)
        : base(createData.FullName, createData.PhoneNumber, createData.BirthDate, customerId,
            createData.Category, requestContent, extraElements) { }

    public static Result<GeneralSupportRequest> Create(string firstName, string lastName,
        string middleName, string phoneNumber, DateOnly? birthDate, Guid customerId, string category,
        string content, Dictionary<string, object> extraElements)
    {
        Result<SupportRequestCreateData> validateData = ValidateCreateData(firstName, lastName, middleName, phoneNumber,
            birthDate, customerId, category, content);

        if (validateData.IsFailure)
        {
            return Result<GeneralSupportRequest>.Failure(validateData);
        }

        GeneralSupportRequest supportRequest = new(validateData.Value!, customerId, content, extraElements);
        return Result<GeneralSupportRequest>.Success(supportRequest);
    }
}