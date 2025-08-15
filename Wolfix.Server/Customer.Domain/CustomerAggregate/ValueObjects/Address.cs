using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate.ValueObjects;

internal sealed class Address
{
    public string City { get; private set; }
    
    public string Street { get; private set; }
    
    public uint HouseNumber { get; private set; }
    
    public uint? ApartmentNumber { get; private set; }

    private Address(string city, string street, uint houseNumber, uint? apartmentNumber = null)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        ApartmentNumber = apartmentNumber;
    }
    
    public static Result<Address> Create(string city, string street, uint houseNumber, uint? apartmentNumber = null)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<Address>.Failure($"{nameof(city)} cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<Address>.Failure($"{nameof(street)} cannot be null or empty");
        }

        Address address = new(city, street, houseNumber, apartmentNumber);
        return Result<Address>.Success(address);
    }

    public override string ToString()
    {
        string apartmentNumber = ApartmentNumber.HasValue ? $", кв.{ApartmentNumber}" : string.Empty;
        return $"м.{City}, вул.{Street}, буд.{HouseNumber}{apartmentNumber}";
    }
}