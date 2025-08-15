using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.ValueObjects;

internal sealed class BirthDate
{
    public DateOnly Value { get; private set; }

    private BirthDate(DateOnly value)
    {
        Value = value;
    }

    public static Result<BirthDate> Create(DateOnly value)
    {
        DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);
        
        if (value.Year > now.Year)
        {
            return Result<BirthDate>.Failure($"year cannot be in the future");
        }
        
        if (value.Year == now.Year && value.Month > now.Month)
        {
            return Result<BirthDate>.Failure($"month cannot be in the future");
        }
        
        if (value.Year == now.Year && value.Month == now.Month && value.Day > now.Day)
        {
            return Result<BirthDate>.Failure($"day cannot be in the future");
        }

        BirthDate birthDate = new(value);
        return Result<BirthDate>.Success(birthDate);
    }
    
    public override string ToString()
    {
        return Value.ToString("dd.MM.yyyy");
    }

    public static explicit operator BirthDate(string dateOnlyString)
    {
        Result<BirthDate> createBirthDateResult = Create(DateOnly.Parse(dateOnlyString));

        if (!createBirthDateResult.IsSuccess)
        {
            throw new ArgumentException(createBirthDateResult.ErrorMessage, nameof(dateOnlyString));
        }
        
        return createBirthDateResult.Value!;
    }
}