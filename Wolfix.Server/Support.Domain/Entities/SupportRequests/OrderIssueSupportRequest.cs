using System.Runtime.CompilerServices;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Support.Domain.Enums;
using Support.Domain.Extensions;

namespace Support.Domain.Entities.SupportRequests;

public sealed class OrderIssueSupportRequest : BaseSupportRequest
{
    public Guid OrderId { get; private set; }
    
    public string OrderNumber { get; private set; }

    private OrderIssueSupportRequest() { }

    private OrderIssueSupportRequest(SupportRequestCreateData createData, Guid customerId,
        string requestContent, Dictionary<string, object> extraElements, Guid orderId, string orderNumber)
        : base(createData.FullName, createData.PhoneNumber, createData.BirthDate, customerId,
            createData.Category, requestContent, extraElements)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
    }

    public static Result<OrderIssueSupportRequest> Create(string firstName, string lastName, string middleName,
        string phoneNumber, DateOnly? birthDate, Guid customerId, string category, string content,
        Dictionary<string, object> extraElements, Guid orderId, string orderNumber)
    {
        if (orderId == Guid.Empty)
        {
            return Result<OrderIssueSupportRequest>.Failure("order id cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            return Result<OrderIssueSupportRequest>.Failure("order number cannot be null or empty");
        }

        Result<SupportRequestCreateData> validateData = ValidateCreateData(firstName, lastName, middleName, phoneNumber,
            birthDate, customerId, category, content);

        if (validateData.IsFailure)
        {
            return Result<OrderIssueSupportRequest>.Failure(validateData);
        }

        OrderIssueSupportRequest supportRequest = new(validateData.Value!, customerId, content, extraElements, orderId, orderNumber);
        return Result<OrderIssueSupportRequest>.Success(supportRequest);
    }
}