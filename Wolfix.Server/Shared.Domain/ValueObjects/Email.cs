using System.Text.RegularExpressions;
using Shared.Domain.Models;

namespace Shared.Domain.ValueObjects;

public sealed class Email
{
    private static readonly Regex Regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    
    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure($"{nameof(value)} cannot be null or empty");
        }

        if (!Regex.IsMatch(value))
        {
            return Result<Email>.Failure($"{nameof(value)} is not valid");
        }

        return Result<Email>.Success(new(value));
    }

    public static explicit operator Email(string emailString)
    {
        Result<Email> createEmailResult = Create(emailString);
        
        if (!createEmailResult.IsSuccess)
        {
            throw new ArgumentException(createEmailResult.ErrorMessage, nameof(emailString));
        }
        
        return createEmailResult.Value!;
    }

    public static explicit operator string(Email email)
    {
        return email.Value;
    }
}