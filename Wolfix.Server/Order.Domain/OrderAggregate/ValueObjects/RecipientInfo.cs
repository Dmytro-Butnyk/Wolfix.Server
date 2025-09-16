using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Order.Domain.OrderAggregate.ValueObjects;

internal sealed class RecipientInfo
{
    public FullName FullName { get; }
    
    public PhoneNumber PhoneNumber { get; }

    private RecipientInfo(FullName fullName, PhoneNumber phoneNumber)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    public static Result<RecipientInfo> Create(string firstName, string lastName, string middleName, string phoneNumber)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<RecipientInfo>.Failure(createFullNameResult);
        }
        
        var fullName = createFullNameResult.Value!;
        
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);

        if (createPhoneNumberResult.IsFailure)
        {
            return Result<RecipientInfo>.Failure(createPhoneNumberResult);
        }

        var phoneNumberValueObject = createPhoneNumberResult.Value!;
        
        return Result<RecipientInfo>.Success(new(fullName, phoneNumberValueObject));
    }
}