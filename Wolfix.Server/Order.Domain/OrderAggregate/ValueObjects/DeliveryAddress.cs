using Shared.Domain.Models;

namespace Order.Domain.OrderAggregate.ValueObjects;

internal sealed class DeliveryAddress
{
    public uint? Number { get; }
    
    public string City { get; }
    
    public string Street { get; }
    
    public uint HouseNumber { get; }
    
    public string? Note { get; }

    private DeliveryAddress(string city, string street, uint houseNumber, uint? number, string? note)
    {
        Number = number;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        Note = note;
    }

    public static Result<DeliveryAddress> ForDepartment(string city, string street, uint houseNumber, uint number)
    {
        if (number <= 0)
        {
            return Result<DeliveryAddress>.Failure($"{nameof(number)} must be positive");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(city)} cannot be null or empty");
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(street)} cannot be null or empty");
        }
        
        if (houseNumber <= 0)
        {
            return Result<DeliveryAddress>.Failure($"{nameof(houseNumber)} must be positive");
        }
        
        return Result<DeliveryAddress>.Success(new(city, street, houseNumber, number,null));
    }

    public static Result<DeliveryAddress> ForPostMachine(string city, string street, uint houseNumber,
        uint number, string? note)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(city)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(street)} cannot be null or empty");
        }
        
        if (houseNumber <= 0)
        {
            return Result<DeliveryAddress>.Failure($"{nameof(houseNumber)} must be positive");
        }
        
        if (number <= 0)
        {
            return Result<DeliveryAddress>.Failure($"{nameof(number)} must be positive");
        }
        
        return Result<DeliveryAddress>.Success(new(city, street, houseNumber, number, note));
    }

    public static Result<DeliveryAddress> ForCourier(string city, string street, uint houseNumber)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(city)} cannot be null or empty");
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<DeliveryAddress>.Failure($"{nameof(street)} cannot be null or empty");
        }
        
        if (houseNumber <= 0)
        {
            return Result<DeliveryAddress>.Failure($"{nameof(houseNumber)} must be positive");
        }
        
        return Result<DeliveryAddress>.Success(new(city, street, houseNumber, null, null));
    }
}