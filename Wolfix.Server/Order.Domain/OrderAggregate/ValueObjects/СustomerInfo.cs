using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Order.Domain.OrderAggregate.ValueObjects;

internal sealed class CustomerInfo
{
    public FullName FullName { get; }
    
    public PhoneNumber PhoneNumber { get; }
    
    public Email Email { get; }
    
    public decimal BonusesAmount { get; }

    private CustomerInfo(FullName fullName, PhoneNumber phoneNumber, Email email)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public static Result<CustomerInfo> Create(string firstName, string lastName, string middleName,
        string phoneNumber, string email)
    {
        var fullNameResult = FullName.Create(firstName, lastName, middleName);
        
        if (fullNameResult.IsFailure)
        {
            return Result<CustomerInfo>.Failure(fullNameResult);
        }

        var phoneNumberResult = PhoneNumber.Create(phoneNumber);
        
        if (phoneNumberResult.IsFailure)
        {
            return Result<CustomerInfo>.Failure(phoneNumberResult);
        }

        var emailResult = Email.Create(email);
        
        if (emailResult.IsFailure)
        {
            return Result<CustomerInfo>.Failure(emailResult);
        }

        return Result<CustomerInfo>.Success(
            new CustomerInfo(fullNameResult.Value!, phoneNumberResult.Value!, emailResult.Value!)
        );
    }
}