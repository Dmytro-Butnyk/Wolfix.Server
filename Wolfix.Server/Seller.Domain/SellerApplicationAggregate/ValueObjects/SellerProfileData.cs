using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Seller.Domain.SellerApplicationAggregate.ValueObjects;

public sealed class SellerProfileData
{
    public FullName FullName { get; }

    public PhoneNumber PhoneNumber { get; }
    
    public Address Address { get; }
    
    public BirthDate BirthDate { get; }
    
    private SellerProfileData() { }

    private SellerProfileData(FullName fullName, PhoneNumber phoneNumber, Address address, BirthDate birthDate)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        BirthDate = birthDate;   
    }

    public static Result<SellerProfileData> Create(string firstName, string lastName, string middleName,
        string phoneNumber, string city, string street, uint houseNumber, uint? apartmentNumber, DateOnly birthDate)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (createFullNameResult.IsFailure)
        {
            return Result<SellerProfileData>.Failure(createFullNameResult);
        }

        FullName fullName = createFullNameResult.Value!;

        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);

        if (createPhoneNumberResult.IsFailure)
        {
            return Result<SellerProfileData>.Failure(createPhoneNumberResult);
        }

        PhoneNumber createdPhoneNumber = createPhoneNumberResult.Value!;

        Result<Address> createAddressResult = Address.Create(city, street, houseNumber, apartmentNumber);

        if (createAddressResult.IsFailure)
        {
            return Result<SellerProfileData>.Failure(createAddressResult);
        }

        Address address = createAddressResult.Value!;

        Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate);

        if (createBirthDateResult.IsFailure)
        {
            return Result<SellerProfileData>.Failure(createBirthDateResult);
        }

        BirthDate createdBirthDate = createBirthDateResult.Value!;
        
        return Result<SellerProfileData>.Success(new(fullName, createdPhoneNumber, address, createdBirthDate));
    }
}