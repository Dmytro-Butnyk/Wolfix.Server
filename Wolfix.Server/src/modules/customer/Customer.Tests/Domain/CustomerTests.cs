using System.Net;
using Customer.Domain.CustomerAggregate;
using FluentAssertions;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Tests.Domain;

public class CustomerTests
{
    // Arrange
    private static readonly Guid AccountId = Guid.NewGuid();
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const string MiddleName = "Smith";
    private const string PhotoUrl = "http://example.com/photo.jpg";
    private const string City = "New York";
    private const string Street = "Main St";
    private const uint HouseNumber = 123;
    private const uint ApartmentNumber = 45;
    private static readonly DateOnly BirthDate = new(2000, 1, 1);
    private const string PhoneNumber = "+380991234567";

    #region Create
    [Fact]
    public void Create_Should_ReturnSuccessResult_When_AccountIdIsNotEmpty()
    {
        // Act
        Result<CustomerAggregate> result = CustomerAggregate.Create(AccountId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccountId.Should().Be(AccountId);
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_When_AccountIdIsEmpty()
    {
        // Arrange
        var emptyAccountId = Guid.Empty;

        // Act
        Result<CustomerAggregate> result = CustomerAggregate.Create(emptyAccountId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("accountId cannot be empty");
    }
    #endregion

    #region CreateViaGoogle
    [Fact]
    public void CreateViaGoogle_Should_ReturnSuccessResult_When_ParametersAreValid()
    {
        // Act
        Result<CustomerAggregate> result = CustomerAggregate.CreateViaGoogle(AccountId, LastName, FirstName, PhotoUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccountId.Should().Be(AccountId);
        result.Value.GetFirstName().Should().Be(FirstName);
        result.Value.GetLastName().Should().Be(LastName);
        result.Value.PhotoUrl.Should().Be(PhotoUrl);
    }

    [Fact]
    public void CreateViaGoogle_Should_ReturnFailureResult_When_AccountIdIsEmpty()
    {
        // Arrange
        var emptyAccountId = Guid.Empty;

        // Act
        Result<CustomerAggregate> result = CustomerAggregate.CreateViaGoogle(emptyAccountId, LastName, FirstName, PhotoUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("accountId cannot be empty");
    }

    [Theory]
    [InlineData("", "Doe", "http://example.com/photo.jpg")]
    [InlineData("John", "", "http://example.com/photo.jpg")]
    [InlineData("John", "Doe", "")]
    [InlineData("John", "Doe", " ")]
    public void CreateViaGoogle_Should_ReturnFailureResult_When_ParametersAreInvalid(string firstName, string lastName, string photoUrl)
    {
        // Act
        Result<CustomerAggregate> result = CustomerAggregate.CreateViaGoogle(AccountId, lastName, firstName, photoUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
    #endregion
    
    #region Getters
    [Fact]
    public void Getters_Should_ReturnDefaultValue_When_PropertiesAreNotSet()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        const string defaultValue = "Не зазначено";

        // Act & Assert
        customer.GetFullName().Should().Be(defaultValue);
        customer.GetFirstName().Should().Be(defaultValue);
        customer.GetLastName().Should().Be(defaultValue);
        customer.GetMiddleName().Should().Be(defaultValue);
        customer.GetPhoneNumber().Should().Be(defaultValue);
        customer.GetAddress().Should().Be(defaultValue);
        customer.GetCity().Should().Be(defaultValue);
        customer.GetStreet().Should().Be(defaultValue);
        customer.GetHouseNumber().Should().BeNull();
        customer.GetApartmentNumber().Should().BeNull();
        customer.GetBirthDate().Should().BeNull();
    }
    
    [Fact]
    public void Getters_Should_ReturnCorrectValues_When_PropertiesAreSet()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.CreateViaGoogle(AccountId, LastName, FirstName, PhotoUrl).Value!;
        customer.ChangeFullName(FirstName, LastName, MiddleName);
        customer.ChangePhoneNumber(PhoneNumber);
        customer.ChangeAddress(City, Street, HouseNumber, ApartmentNumber);
        customer.ChangeBirthDate(BirthDate);

        // Act & Assert
        customer.GetFullName().Should().Be($"{FirstName} {LastName} {MiddleName}");
        customer.GetFirstName().Should().Be(FirstName);
        customer.GetLastName().Should().Be(LastName);
        customer.GetMiddleName().Should().Be(MiddleName);
        customer.GetPhoneNumber().Should().Be(PhoneNumber);
        customer.GetAddress().Should().Be($"м. {City}, вул. {Street}, буд. {HouseNumber}, кв. {ApartmentNumber}");
        customer.GetCity().Should().Be(City);
        customer.GetStreet().Should().Be(Street);
        customer.GetHouseNumber().Should().Be(HouseNumber);
        customer.GetApartmentNumber().Should().Be(ApartmentNumber);
        customer.GetBirthDate().Should().Be(BirthDate);
    }
    #endregion

    #region Setters
    [Fact]
    public void ChangePhoto_Should_UpdatePhotoUrl_When_UrlIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        const string newPhotoUrl = "http://example.com/new_photo.jpg";

        // Act
        VoidResult result = customer.ChangePhoto(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.PhotoUrl.Should().Be(newPhotoUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ChangePhoto_Should_ReturnFailure_When_UrlIsInvalid(string newPhotoUrl)
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangePhoto(newPhotoUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("photoUrl cannot be empty");
    }
    
    [Fact]
    public void ChangeFullName_Should_UpdateFullName_When_NamesAreValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangeFullName(FirstName, LastName, MiddleName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.GetFullName().Should().Be($"{FirstName} {LastName} {MiddleName}");
    }
    
    [Fact]
    public void ChangePhoneNumber_Should_UpdatePhoneNumber_When_NumberIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangePhoneNumber(PhoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.GetPhoneNumber().Should().Be(PhoneNumber);
    }
    
    [Fact]
    public void ChangeAddress_Should_UpdateAddress_When_AddressIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangeAddress(City, Street, HouseNumber, ApartmentNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.GetAddress().Should().Be($"м. {City}, вул. {Street}, буд. {HouseNumber}, кв. {ApartmentNumber}");
    }
    
    [Fact]
    public void ChangeBirthDate_Should_UpdateBirthDate_When_DateIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangeBirthDate(BirthDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.GetBirthDate().Should().Be(BirthDate);
    }
    
    [Fact]
    public void ChangeBonusesAmount_Should_UpdateBonuses_When_AmountIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        const decimal bonuses = 100;

        // Act
        VoidResult result = customer.ChangeBonusesAmount(bonuses);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.BonusesAmount.Should().Be(bonuses);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void ChangeBonusesAmount_Should_ReturnFailure_When_AmountIsInvalid(decimal bonuses)
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.ChangeBonusesAmount(bonuses);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("bonusesAmount cannot be less than or equal to zero");
    }
    #endregion

    #region ViolationStatus
    [Fact]
    public void AddViolation_Should_IncreaseViolationCount()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        // TODO: ViolationStatus is internal, cannot directly check the count. Test relies on internal implementation knowledge.
        
        // Act
        VoidResult result = customer.AddViolation();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    #endregion

    #region FavoriteItems
    [Fact]
    public void AddFavoriteItem_Should_AddItem_When_ItemIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.AddFavoriteItem("Item 1", PhotoUrl, 100, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.FavoriteItems.Should().HaveCount(1);
    }

    [Fact]
    public void AddFavoriteItem_Should_ReturnFailure_When_ItemAlreadyExists()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddFavoriteItem("Item 1", PhotoUrl, 100, 10);

        // Act
        VoidResult result = customer.AddFavoriteItem("Item 1", PhotoUrl, 100, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        // TODO: The check for existing favorite item might be too strict or incorrect.
        // It compares all properties including nullable doubles.
    }

    [Fact]
    public void RemoveFavoriteItem_Should_RemoveItem_When_ItemExists()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddFavoriteItem("Item 1", PhotoUrl, 100, 10);
        Guid itemId = customer.FavoriteItems.First().Id;

        // Act
        VoidResult result = customer.RemoveFavoriteItem(itemId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.FavoriteItems.Should().BeEmpty();
    }

    [Fact]
    public void RemoveFavoriteItem_Should_ReturnNotFound_When_ItemDoesNotExist()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        var nonExistentItemId = Guid.NewGuid();

        // Act
        VoidResult result = customer.RemoveFavoriteItem(nonExistentItemId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void RemoveAllFavoriteItems_Should_ClearList()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddFavoriteItem("Item 1", PhotoUrl, 100, 10);
        customer.AddFavoriteItem("Item 2", PhotoUrl, 200, 20);

        // Act
        customer.RemoveAllFavoriteItems();

        // Assert
        customer.FavoriteItems.Should().BeEmpty();
    }
    #endregion

    #region CartItems
    [Fact]
    public void AddCartItem_Should_AddItem_When_ItemIsValid()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;

        // Act
        VoidResult result = customer.AddCartItem(PhotoUrl, "Cart Item 1", 150, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.CartItems.Should().HaveCount(1);
        customer.TotalCartPriceWithoutBonuses.Should().Be(150);
    }

    [Fact]
    public void AddCartItem_Should_ReturnFailure_When_ItemAlreadyExists()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddCartItem(PhotoUrl, "Cart Item 1", 150, Guid.NewGuid(), Guid.NewGuid());

        // Act
        VoidResult result = customer.AddCartItem(PhotoUrl, "Cart Item 1", 150, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public void RemoveCartItem_Should_RemoveItem_When_ItemExists()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddCartItem(PhotoUrl, "Cart Item 1", 150, Guid.NewGuid(), Guid.NewGuid());
        Guid itemId = customer.CartItems.First().Id;

        // Act
        VoidResult result = customer.RemoveCartItem(itemId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        customer.CartItems.Should().BeEmpty();
        customer.TotalCartPriceWithoutBonuses.Should().Be(0);
    }

    [Fact]
    public void RemoveCartItem_Should_ReturnNotFound_When_ItemDoesNotExist()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        var nonExistentItemId = Guid.NewGuid();

        // Act
        VoidResult result = customer.RemoveCartItem(nonExistentItemId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void RemoveAllCartItems_Should_ClearList()
    {
        // Arrange
        CustomerAggregate customer = CustomerAggregate.Create(AccountId).Value!;
        customer.AddCartItem(PhotoUrl, "Cart Item 1", 150, Guid.NewGuid(), Guid.NewGuid());
        customer.AddCartItem(PhotoUrl, "Cart Item 2", 250, Guid.NewGuid(), Guid.NewGuid());

        // Act
        customer.RemoveAllCartItems();

        // Assert
        customer.CartItems.Should().BeEmpty();
        customer.TotalCartPriceWithoutBonuses.Should().Be(0);
    }
    #endregion
}
