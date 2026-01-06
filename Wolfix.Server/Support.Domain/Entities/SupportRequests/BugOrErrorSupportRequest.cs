using Shared.Domain.Models;

namespace Support.Domain.Entities.SupportRequests;

public sealed class BugOrErrorSupportRequest : BaseSupportRequest
{
    public string PhotoUrl { get; private set; }

    private BugOrErrorSupportRequest() { }

    private BugOrErrorSupportRequest(SupportRequestCreateData createData, Guid customerId,
        string requestContent, Dictionary<string, object> extraElements, string photoUrl)
        : base(createData.FullName, createData.PhoneNumber, createData.BirthDate, customerId,
            createData.Category, requestContent, extraElements)
    {
        PhotoUrl = photoUrl;
    }

    public static Result<BugOrErrorSupportRequest> Create(string firstName, string lastName,
        string middleName, string phoneNumber, DateOnly? birthDate, Guid customerId, string category,
        string content, Dictionary<string, object> extraElements, string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Result<BugOrErrorSupportRequest>.Failure("photo url cannot be null or white space");
        }
        
        Result<SupportRequestCreateData> validateData = ValidateCreateData(firstName, lastName, middleName, phoneNumber,
            birthDate, customerId, category, content);

        if (validateData.IsFailure)
        {
            return Result<BugOrErrorSupportRequest>.Failure(validateData);
        }

        BugOrErrorSupportRequest supportRequest = new(validateData.Value!, customerId, content, extraElements, photoUrl);
        return Result<BugOrErrorSupportRequest>.Success(supportRequest);
    }
}