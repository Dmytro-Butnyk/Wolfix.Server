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

    public static explicit operator Address(string addressString)
    {
        if (string.IsNullOrWhiteSpace(addressString))
        {
            throw new ArgumentException("Address string cannot be null or empty.");
        }

        try
        {
            string[] parts = addressString.Split(',', StringSplitOptions.TrimEntries);

            string city = parts.First(p => p.StartsWith("м.")).Replace("м.", "").Trim();
            string street = parts.First(p => p.StartsWith("вул.")).Replace("вул.", "").Trim();

            string housePart = parts.First(p => p.StartsWith("буд."));
            uint houseNumber = uint.Parse(housePart.Replace("буд.", "").Trim());

            uint? apartmentNumber = null;
            string? apartmentPart = parts.FirstOrDefault(p => p.StartsWith("кв."));
            if (apartmentPart != null)
            {
                apartmentNumber = uint.Parse(apartmentPart.Replace("кв.", "").Trim());
            }

            return Create(city, street, houseNumber, apartmentNumber).Value!;
        }
        catch
        {
            throw new FormatException("Invalid address string format.");
        }
    }
}