using Customer.Domain.CustomerAggregate.Entities;
using Customer.Domain.CustomerAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate;

public sealed class Customer : BaseEntity
{
    internal FullName FullName { get; private set; }
    
    internal PhoneNumber PhoneNumber { get; private set; }
    
    internal Address Address { get; private set; }
    
    internal BirthDate BirthDate { get; private set; }
    
    public decimal BonusesAmount { get; private set; }

    private readonly List<FavoriteItem> _favoriteItems = [];
    public IReadOnlyCollection<FavoriteItemInfo> FavoriteItems => _favoriteItems
        .Select(fi => new FavoriteItemInfo(fi.CustomerId, fi.PhotoUrl, fi.Title, fi.AverageRating,
            fi.Price, fi.FinalPrice, fi.Bonuses))
        .ToList()
        .AsReadOnly();

    private readonly List<CartItem> _cartItems = [];
    public IReadOnlyCollection<CartItemInfo> CartItems => _cartItems
        .Select(ci => new CartItemInfo(ci.CustomerId, ci.PhotoUrl, ci.Title, ci.Price))
        .ToList()
        .AsReadOnly();

    private Customer() { }

    private Customer(FullName fullName, PhoneNumber phoneNumber, Address address,
        BirthDate birthDate, decimal bonusesAmount)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        BirthDate = birthDate;
        BonusesAmount = bonusesAmount;
    }

    public static Result<Customer> Create(string firstName, string lastName, string middleName, string phoneNumberString,
        string city, string street, uint houseNumber, uint? apartmentNumber, DateOnly birthDate, decimal bonusesAmount)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);
        
        if (!createFullNameResult.IsSuccess)
        {
            return Result<Customer>.Failure(createFullNameResult.ErrorMessage!);
        }
        
        FullName fullName = createFullNameResult.Value!;
        
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumberString);

        if (!createPhoneNumberResult.IsSuccess)
        {
            return Result<Customer>.Failure(createPhoneNumberResult.ErrorMessage!);
        }
        
        PhoneNumber phoneNumber = createPhoneNumberResult.Value!;
        
        Result<Address> createAddressResult = Address.Create(city, street, houseNumber, apartmentNumber);
        
        if (!createAddressResult.IsSuccess)
        {
            return Result<Customer>.Failure(createAddressResult.ErrorMessage!);
        }
        
        Address address = createAddressResult.Value!;
        
        Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate);
        
        if (!createBirthDateResult.IsSuccess)
        {
            return Result<Customer>.Failure(createBirthDateResult.ErrorMessage!);
        }
        
        BirthDate customerBirthDate = createBirthDateResult.Value!;

        if (bonusesAmount <= 0)
        {
            return Result<Customer>.Failure($"{nameof(bonusesAmount)} cannot be less than or equal to zero");
        }

        Customer customer = new(fullName, phoneNumber, address, customerBirthDate, bonusesAmount);
        return Result<Customer>.Success(customer);
    }
    
    //todo: change methods and methods for lists
}