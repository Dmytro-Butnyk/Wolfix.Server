using Order.Domain.OrderAggregate.Enums;
using Shared.Domain.Models;

namespace Order.Domain.OrderAggregate.ValueObjects;

internal sealed class DeliveryInfo
{
    public uint? Number { get; }
    
    public string City { get; }
    
    public string Street { get; }
    
    public uint HouseNumber { get; }
    
    public DeliveryOption Option { get; }

    private DeliveryInfo(string city, string street, uint houseNumber, uint? number, DeliveryOption option)
    {
        Number = number;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
    }

    public static Result<DeliveryInfo> Create(string city, string street, uint houseNumber, uint? number, DeliveryOption option)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<DeliveryInfo>.Failure($"{nameof(city)} cannot be null or empty");
        }
        
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<DeliveryInfo>.Failure($"{nameof(street)} cannot be null or empty");
        }

        if (houseNumber <= 0)
        {
            return Result<DeliveryInfo>.Failure($"{nameof(houseNumber)} must be positive");
        }

        if (option == DeliveryOption.Courier)
        {
            return Result<DeliveryInfo>.Success(new(city, street, houseNumber, null, option));
        }

        if (number == null)
        {
            return Result<DeliveryInfo>.Failure($"{nameof(number)} cannot be null");
        }
        
        return Result<DeliveryInfo>.Success(new(city, street, houseNumber, number, option));
    }
}