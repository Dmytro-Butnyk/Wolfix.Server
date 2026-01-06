using System.Runtime.CompilerServices;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Support.Domain.Enums;
using Support.Domain.Extensions;

namespace Support.Domain.Entities.SupportRequests;

public sealed class OrderIssueSupportRequest : BaseSupportRequest
{
    public Guid OrderId { get; private set; }

    private OrderIssueSupportRequest() { }

    private OrderIssueSupportRequest(SupportRequestCreateData createData, Guid customerId,
        string requestContent, Dictionary<string, object> extraElements, Guid orderId)
        : base(createData.FullName, createData.PhoneNumber, createData.BirthDate, customerId,
            createData.Category, requestContent, extraElements)
    {
        OrderId = orderId;
    }

    public static Result<OrderIssueSupportRequest> Create(string firstName, string lastName, string middleName, string phoneNumber,
        DateOnly? birthDate, Guid customerId, string category, string content, Dictionary<string, object> extraElements, Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            return Result<OrderIssueSupportRequest>.Failure("order id cannot be empty");
        }

        Result<SupportRequestCreateData> validateData = ValidateCreateData(firstName, lastName, middleName, phoneNumber,
            birthDate, customerId, category, content);

        if (validateData.IsFailure)
        {
            return Result<OrderIssueSupportRequest>.Failure(validateData);
        }

        OrderIssueSupportRequest supportRequest = new(validateData.Value!, customerId, content, extraElements, orderId);
        return Result<OrderIssueSupportRequest>.Success(supportRequest);
    }
}