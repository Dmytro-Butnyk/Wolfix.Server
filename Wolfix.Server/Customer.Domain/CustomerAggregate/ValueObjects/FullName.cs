using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.ValueObjects;

internal sealed class FullName
{
    public string FirstName { get; private set; }
    
    public string LastName { get; private set; }
    
    public string MiddleName { get; private set; }

    private FullName(string firstName, string lastName, string middleName)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public static Result<FullName> Create(string firstName, string lastName, string middleName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<FullName>.Failure($"{nameof(firstName)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<FullName>.Failure($"{nameof(lastName)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(middleName))
        {
            return Result<FullName>.Failure($"{nameof(middleName)} cannot be null or empty");
        }
        
        FullName fullName = new(firstName, lastName, middleName);
        return Result<FullName>.Success(fullName);
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName} {MiddleName}";
    }
}