using MongoDB.Bson.Serialization.Attributes;
using Shared.Domain.Models;

namespace Shared.Domain.ValueObjects;

public sealed class FullName
{
    public string FirstName { get; init; }
    
    public string LastName { get; init; }
    
    public string? MiddleName { get; init; }

    [BsonConstructor]
    private FullName(string firstName, string lastName, string? middleName = null)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public static Result<FullName> Create(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<FullName>.Failure($"{nameof(firstName)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<FullName>.Failure($"{nameof(lastName)} cannot be null or empty");
        }

        if (middleName is not null)
        {
            if (string.IsNullOrWhiteSpace(middleName))
            {
                return Result<FullName>.Failure($"{nameof(middleName)} cannot be null or empty");
            }
        }

        FullName fullName = new(firstName, lastName, middleName);
        return Result<FullName>.Success(fullName);
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName} {MiddleName}";
    }

    public static explicit operator FullName(string fullNameString)
    {
        if (string.IsNullOrWhiteSpace(fullNameString))
        {
            throw new ArgumentException("Full name string cannot be null or empty.", nameof(fullNameString));
        }

        string[] parts = fullNameString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            throw new ArgumentException("Full name must consist of first name, last name, and middle name separated by spaces.");
        }

        return new FullName(parts[0], parts[1], parts[2]);
    }
}