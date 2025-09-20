using System.Net;
using Seller.Domain.Enums;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Seller.Domain.SellerAggregate;

public sealed class Seller : BaseEntity
{
    public string? PhotoUrl { get; private set; }
    
    internal FullName FullName { get; private set; }

    internal PhoneNumber PhoneNumber { get; private set; }
    
    internal Address Address { get; private set; }
    
    internal BirthDate BirthDate { get; private set; }

    public Guid AccountId { get; private set; }

    private readonly List<SellerCategory> _sellerCategories = [];
    public IReadOnlyCollection<SellerCategoryInfo> SellerCategories => _sellerCategories
        .Select(sc => (SellerCategoryInfo)sc)
        .ToList()
        .AsReadOnly();
    
    private Seller() { }
    
    private Seller(Guid accountId, FullName fullName, PhoneNumber phoneNumber,
        Address address, BirthDate birthDate)
    {
        AccountId = accountId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        BirthDate = birthDate;
    }
    
    public static Result<Seller> Create(Guid accountId, string firstName, string lastName, string middleName,
        string phoneNumber, string city, string street, uint houseNumber, uint? apartmentNumber, DateOnly birthDate)
    {
        if (accountId == Guid.Empty)
        {
            return Result<Seller>.Failure($"{nameof(accountId)} cannot be empty");
        }
        
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);

        if (!createFullNameResult.IsSuccess)
        {
            return Result<Seller>.Failure(createFullNameResult.ErrorMessage!, createFullNameResult.StatusCode);
        }
        FullName newFullName = createFullNameResult.Value!;
        
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);

        if (!createPhoneNumberResult.IsSuccess)
        {
            return Result<Seller>.Failure(createPhoneNumberResult.ErrorMessage!, createPhoneNumberResult.StatusCode);
        }
        PhoneNumber newPhoneNumber = createPhoneNumberResult.Value!;
        
        Result<Address> createAddressResult = Address.Create(city, street, houseNumber, apartmentNumber);
        
        if (!createAddressResult.IsSuccess)
        {
            return Result<Seller>.Failure(createAddressResult.ErrorMessage!, createAddressResult.StatusCode);
        }
        Address newAddress = createAddressResult.Value!;
        
        Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate);

        if (!createBirthDateResult.IsSuccess)
        {
            return Result<Seller>.Failure(createBirthDateResult.ErrorMessage!, createBirthDateResult.StatusCode);
        }
        BirthDate newBirthDate = createBirthDateResult.Value!;

        Seller seller = new(accountId, newFullName, newPhoneNumber, newAddress, newBirthDate);
        return Result<Seller>.Success(seller);
    }
    
    #region Getters
    public string GetFullName()
        => FullName.ToString();
    
    public string GetFirstName()
        => FullName.FirstName;

    public string GetLastName()
        => FullName.LastName;

    public string GetMiddleName()
        => FullName.MiddleName;

    public string GetPhoneNumber()
        => PhoneNumber.Value;

    public string GetAddress()
        => Address.ToString();
    
    public string GetCity()
        => Address.City;

    public string GetStreet()
        => Address.Street;

    public uint? GetHouseNumber()
        => Address.HouseNumber;
    
    public uint? GetApartmentNumber()
        => Address.ApartmentNumber;
    
    public DateOnly? GetBirthDate()
        => BirthDate.Value;
    #endregion
    
    #region Setters
    public VoidResult ChangePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return VoidResult.Failure($"{nameof(photoUrl)} cannot be empty");
        }
        
        PhotoUrl = photoUrl;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeFullName(string firstName, string lastName, string middleName)
    {
        Result<FullName> createFullNameResult = FullName.Create(firstName, lastName, middleName);
        
        if (!createFullNameResult.IsSuccess)
        {
            return VoidResult.Failure(createFullNameResult.ErrorMessage!, createFullNameResult.StatusCode);
        }
        
        FullName = createFullNameResult.Value!;
        return VoidResult.Success();
    }
    
    public VoidResult ChangePhoneNumber(string phoneNumber)
    {
        Result<PhoneNumber> createPhoneNumberResult = PhoneNumber.Create(phoneNumber);

        if (!createPhoneNumberResult.IsSuccess)
        {
            return VoidResult.Failure(createPhoneNumberResult.ErrorMessage!, createPhoneNumberResult.StatusCode);
        }
        
        PhoneNumber = createPhoneNumberResult.Value!;
        return VoidResult.Success();
    }

    public VoidResult ChangeAddress(string city, string street, uint houseNumber, uint? apartmentNumber)
    {
        Result<Address> createAddressResult = Address.Create(city, street, houseNumber, apartmentNumber);
        
        if (!createAddressResult.IsSuccess)
        {
            return VoidResult.Failure(createAddressResult.ErrorMessage!, createAddressResult.StatusCode);
        }
        
        Address = createAddressResult.Value!;
        return VoidResult.Success();
    }

    public VoidResult ChangeBirthDate(DateOnly birthDate)
    {
        Result<BirthDate> createBirthDateResult = BirthDate.Create(birthDate);
        
        if (!createBirthDateResult.IsSuccess)
        {
            return VoidResult.Failure(createBirthDateResult.ErrorMessage!, createBirthDateResult.StatusCode);
        }
        
        BirthDate = createBirthDateResult.Value!;
        return VoidResult.Success();
    }
    #endregion
    
    #region category
    public Result<SellerCategoryInfo> GetSellerCategory(Guid sellerCategoryId)
    {
        SellerCategory? sellerCategory = _sellerCategories.FirstOrDefault(sc => sc.Id == sellerCategoryId);

        if (sellerCategory == null)
        {
            return Result<SellerCategoryInfo>.Failure(
                $"{nameof(sellerCategory)} with id: {sellerCategoryId} does not exist)",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<SellerCategoryInfo>.Success((SellerCategoryInfo)sellerCategory);
    }
    #endregion
    
    #region categories
    public VoidResult AddSellerCategory(Guid categoryId, string name)
    {
        if (_sellerCategories.Any(sc => sc.CategoryId == categoryId))
        {
            return VoidResult.Failure($"{nameof(categoryId)} already exists", HttpStatusCode.Conflict);
        }
        
        Result<SellerCategory> createSellerCategory = SellerCategory.Create(this, categoryId, name);
        
        if (!createSellerCategory.IsSuccess)
        {
            return VoidResult.Failure(createSellerCategory.ErrorMessage!, createSellerCategory.StatusCode);
        }
        
        SellerCategory sellerCategory = createSellerCategory.Value!;
        _sellerCategories.Add(sellerCategory);
        return VoidResult.Success();
    }

    public VoidResult RemoveSellerCategory(Guid sellerCategoryId)
    {
        SellerCategory? sellerCategory = _sellerCategories.FirstOrDefault(sc => sc.Id == sellerCategoryId);

        if (sellerCategory == null)
        {
            return VoidResult.Failure(
                $"{nameof(sellerCategory)} with id: {sellerCategoryId} does not exist",
                HttpStatusCode.NotFound
            );
        }
        
        _sellerCategories.Remove(sellerCategory);
        return VoidResult.Success();
    }
    
    public void RemoveAllSellerCategories()
    {
        _sellerCategories.Clear();
    }
    #endregion
}