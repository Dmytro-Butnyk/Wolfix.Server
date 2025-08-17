using System.Net;
using Customer.Domain.CustomerAggregate.Entities;
using Customer.Domain.CustomerAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Customer.Domain.CustomerAggregate;

public sealed class Customer : BaseEntity
{
    internal FullName? FullName { get; private set; }

    internal PhoneNumber? PhoneNumber { get; private set; }
    
    internal Address? Address { get; private set; }
    
    internal BirthDate? BirthDate { get; private set; }

    public decimal BonusesAmount { get; private set; }
    
    public Guid AccountId { get; private set; }

    private readonly List<FavoriteItem> _favoriteItems = [];
    public IReadOnlyCollection<FavoriteItemInfo> FavoriteItems => _favoriteItems
        .Select(fi => (FavoriteItemInfo)fi)
        .ToList()
        .AsReadOnly();

    private readonly List<CartItem> _cartItems = [];
    public IReadOnlyCollection<CartItemInfo> CartItems => _cartItems
        .Select(ci => (CartItemInfo)ci)
        .ToList()
        .AsReadOnly();

    private Customer() { }

    private Customer(Guid accountId)
    {
        AccountId = accountId;
    }

    public static Result<Customer> Create(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            return Result<Customer>.Failure($"{nameof(accountId)} cannot be empty");
        }

        Customer customer = new(accountId);
        return Result<Customer>.Success(customer);
    }
    
    #region Getters
    private const string DefaultStringValue = "Не зазначено";
    public string GetFullName()
        => FullName == null ? DefaultStringValue : FullName.ToString();
    
    public string GetFirstName()
        => FullName == null ? DefaultStringValue : FullName.FirstName;

    public string GetLastName()
        => FullName == null ? DefaultStringValue : FullName.LastName;

    public string GetMiddleName()
        => FullName == null ? DefaultStringValue : FullName.MiddleName;

    public string GetPhoneNumber()
        => PhoneNumber == null ? DefaultStringValue : PhoneNumber.Value;

    public string GetAddress()
        => Address == null ? DefaultStringValue : Address.ToString();
    
    public string GetCity()
        => Address == null ? DefaultStringValue : Address.City;

    public string GetStreet()
        => Address == null ? DefaultStringValue : Address.Street;

    public uint? GetHouseNumber()
        => Address?.HouseNumber;
    
    public uint? GetApartmentNumber()
        => Address?.ApartmentNumber;
    
    public DateOnly? GetBirthDate()
        => BirthDate?.Value;
    #endregion

    #region Setters
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

    public VoidResult ChangeBonusesAmount(decimal bonusesAmount)
    {
        if (bonusesAmount <= 0)
        {
            return VoidResult.Failure($"{nameof(bonusesAmount)} cannot be less than or equal to zero");
        }
        
        BonusesAmount = bonusesAmount;
        return VoidResult.Success();
    }
    #endregion
    
    #region favoriteItem
    public Result<FavoriteItemInfo> GetFavoriteItem(Guid favoriteItemId)
    {
        FavoriteItem? favoriteItem = _favoriteItems.FirstOrDefault(fi => fi.Id == favoriteItemId);

        if (favoriteItem == null)
        {
            return Result<FavoriteItemInfo>.Failure(
                $"{nameof(favoriteItem)} with id: {favoriteItemId} does not exist",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<FavoriteItemInfo>.Success((FavoriteItemInfo)favoriteItem);
    }
    #endregion
    
    #region favoriteItems
    public VoidResult AddFavoriteItem(string title, string photoUrl, decimal price, uint bonuses,
        double? averageRating = null, decimal? finalPrice = null)
    {
        //todo:?
        if (_favoriteItems.Any(fi => fi.Title == title && fi.PhotoUrl == photoUrl && fi.Price == price &&
                fi.Bonuses == bonuses && fi.AverageRating == averageRating && fi.FinalPrice == finalPrice))
        {
            return VoidResult.Failure("This product already exists in favorite", HttpStatusCode.Conflict);
        }

        Result<FavoriteItem> createFavoriteItemResult =
            FavoriteItem.Create(this, photoUrl, title, price, bonuses, averageRating, finalPrice);
        
        if (!createFavoriteItemResult.IsSuccess)
        {
            return VoidResult.Failure(createFavoriteItemResult.ErrorMessage!, createFavoriteItemResult.StatusCode);;
        }
        
        FavoriteItem favoriteItem = createFavoriteItemResult.Value!;
        _favoriteItems.Add(favoriteItem);
        return VoidResult.Success();
    }
    
    public VoidResult RemoveFavoriteItem(Guid favoriteItemId)
    {
        FavoriteItem? favoriteItem = _favoriteItems.FirstOrDefault(fi => fi.Id == favoriteItemId);
        
        if (favoriteItem == null)
        {
            return VoidResult.Failure(
                $"{nameof(favoriteItem)} with id: {favoriteItemId} does not exist",
                HttpStatusCode.NotFound
            );
        }
        
        _favoriteItems.Remove(favoriteItem);
        return VoidResult.Success();
    }

    public void RemoveAllFavoriteItems()
    {
        _favoriteItems.Clear();
    }
    #endregion
    
    #region cartItem
    public Result<CartItemInfo> GetCartItem(Guid cartItemId)
    {
        CartItem? cartItem = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        
        if (cartItem == null)
        {
            return Result<CartItemInfo>.Failure(
                $"{nameof(cartItem)} with id: {cartItemId} does not exist",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<CartItemInfo>.Success((CartItemInfo)cartItem);
    }
    #endregion
    
    #region cartItems
    public VoidResult AddCartItem(string photoUrl, string title, decimal price)
    {
        if (_cartItems.Any(ci => ci.PhotoUrl == photoUrl && ci.Title == title && ci.Price == price))
        {
            return VoidResult.Failure("This product already exists in cart", HttpStatusCode.Conflict);
        }
        
        Result<CartItem> createCartItemResult = CartItem.Create(this, photoUrl, title, price);
        
        if (!createCartItemResult.IsSuccess)
        {
            return VoidResult.Failure(createCartItemResult.ErrorMessage!);
        }
        
        CartItem cartItem = createCartItemResult.Value!;
        _cartItems.Add(cartItem);
        return VoidResult.Success();
    }

    public VoidResult RemoveCartItem(Guid cartItemId)
    {
        CartItem? cartItem = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        
        if (cartItem == null)
        {
            return VoidResult.Failure(
                $"{nameof(cartItem)} with id: {cartItemId} does not exist",
                HttpStatusCode.NotFound
            );
        }
        
        _cartItems.Remove(cartItem);
        return VoidResult.Success();
    }

    public void RemoveAllCartItems()
    {
        _cartItems.Clear();
    }
    #endregion
}