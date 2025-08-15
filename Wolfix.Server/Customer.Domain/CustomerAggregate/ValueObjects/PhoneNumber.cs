using System.Text.RegularExpressions;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.ValueObjects;

internal sealed class PhoneNumber
{
    private static readonly Regex Regex = new(@"^\+380\d{9}$", RegexOptions.Compiled);
    
    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }
    
    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PhoneNumber>.Failure($"{nameof(value)} cannot be null or empty");
        }

        if (Regex.IsMatch(value))
        {
            return Result<PhoneNumber>.Failure($"{nameof(value)} is not valid");
        }

        PhoneNumber phoneNumber = new(value);
        return Result<PhoneNumber>.Success(phoneNumber);
    }

    public static explicit operator PhoneNumber(string phoneNumberString)
    {
        Result<PhoneNumber> createPhoneNumberResult = Create(phoneNumberString);
        
        if (!createPhoneNumberResult.IsSuccess)
        {
            throw new ArgumentException(createPhoneNumberResult.ErrorMessage, nameof(phoneNumberString));
        }
        
        return createPhoneNumberResult.Value!;
    }
}