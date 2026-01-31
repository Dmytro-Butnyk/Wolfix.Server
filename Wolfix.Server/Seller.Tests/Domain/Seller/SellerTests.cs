using FluentAssertions;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.Models;
using System.Net;
using Xunit;
using SellerEntity = Seller.Domain.SellerAggregate.Seller;

namespace Seller.Tests.Domain.Seller;

public class SellerTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenInputsAreValid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        string city = "Kyiv";
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccountId.Should().Be(accountId);
        result.Value.GetFirstName().Should().Be(firstName);
        result.Value.GetLastName().Should().Be(lastName);
        result.Value.GetMiddleName().Should().Be(middleName);
        result.Value.GetPhoneNumber().Should().Be(phoneNumber);
        result.Value.GetCity().Should().Be(city);
        result.Value.GetStreet().Should().Be(street);
        result.Value.GetHouseNumber().Should().Be(houseNumber);
        result.Value.GetApartmentNumber().Should().Be(apartmentNumber);
        result.Value.GetBirthDate().Should().Be(birthDate);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenAccountIdIsEmpty()
    {
        // Arrange
        Guid accountId = Guid.Empty;
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        string city = "Kyiv";
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("accountId");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFullNameIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = ""; // Invalid
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        string city = "Kyiv";
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "123"; // Invalid
        string city = "Kyiv";
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenAddressIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        string city = ""; // Invalid
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenBirthDateIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        string city = "Kyiv";
        string street = "Khreshchatyk";
        uint houseNumber = 1;
        uint? apartmentNumber = 10;
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)); // Future date

        // Act
        Result<SellerEntity> result = SellerEntity.Create(
            accountId, firstName, lastName, middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ChangePhoto_ShouldUpdatePhotoUrl_WhenUrlIsValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newPhotoUrl = "http://example.com/photo.jpg";

        // Act
        VoidResult result = seller.ChangePhoto(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.PhotoUrl.Should().Be(newPhotoUrl);
    }

    [Fact]
    public void ChangePhoto_ShouldReturnFailure_WhenUrlIsEmpty()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newPhotoUrl = "";

        // Act
        VoidResult result = seller.ChangePhoto(newPhotoUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("photoUrl");
    }

    [Fact]
    public void ChangeFullName_ShouldUpdateFullName_WhenInputsAreValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newFirstName = "Jane";
        string newLastName = "Smith";
        string newMiddleName = "Doe";

        // Act
        VoidResult result = seller.ChangeFullName(newFirstName, newLastName, newMiddleName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.GetFirstName().Should().Be(newFirstName);
        seller.GetLastName().Should().Be(newLastName);
        seller.GetMiddleName().Should().Be(newMiddleName);
    }

    [Fact]
    public void ChangeFullName_ShouldReturnFailure_WhenInputsAreInvalid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newFirstName = "";
        string newLastName = "Smith";
        string newMiddleName = "Doe";

        // Act
        VoidResult result = seller.ChangeFullName(newFirstName, newLastName, newMiddleName);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ChangePhoneNumber_ShouldUpdatePhoneNumber_WhenInputIsValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newPhoneNumber = "+380987654321";

        // Act
        VoidResult result = seller.ChangePhoneNumber(newPhoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.GetPhoneNumber().Should().Be(newPhoneNumber);
    }

    [Fact]
    public void ChangePhoneNumber_ShouldReturnFailure_WhenInputIsInvalid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newPhoneNumber = "invalid";

        // Act
        VoidResult result = seller.ChangePhoneNumber(newPhoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ChangeAddress_ShouldUpdateAddress_WhenInputsAreValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newCity = "Lviv";
        string newStreet = "Rynok Square";
        uint newHouseNumber = 2;
        uint? newApartmentNumber = 20;

        // Act
        VoidResult result = seller.ChangeAddress(newCity, newStreet, newHouseNumber, newApartmentNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.GetCity().Should().Be(newCity);
        seller.GetStreet().Should().Be(newStreet);
        seller.GetHouseNumber().Should().Be(newHouseNumber);
        seller.GetApartmentNumber().Should().Be(newApartmentNumber);
    }

    [Fact]
    public void ChangeAddress_ShouldReturnFailure_WhenInputsAreInvalid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        string newCity = "";
        string newStreet = "Rynok Square";
        uint newHouseNumber = 2;
        uint? newApartmentNumber = 20;

        // Act
        VoidResult result = seller.ChangeAddress(newCity, newStreet, newHouseNumber, newApartmentNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ChangeBirthDate_ShouldUpdateBirthDate_WhenInputIsValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        DateOnly newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));

        // Act
        VoidResult result = seller.ChangeBirthDate(newBirthDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.GetBirthDate().Should().Be(newBirthDate);
    }

    [Fact]
    public void ChangeBirthDate_ShouldReturnFailure_WhenInputIsInvalid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        DateOnly newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));

        // Act
        VoidResult result = seller.ChangeBirthDate(newBirthDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddSellerCategory_ShouldAddCategory_WhenInputIsValid()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string categoryName = "Electronics";

        // Act
        VoidResult result = seller.AddSellerCategory(categoryId, categoryName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.SellerCategories.Should().Contain(sc => sc.CategoryId == categoryId && sc.Name == categoryName);
    }

    [Fact]
    public void AddSellerCategory_ShouldReturnFailure_WhenCategoryAlreadyExists()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string categoryName = "Electronics";
        seller.AddSellerCategory(categoryId, categoryName);

        // Act
        VoidResult result = seller.AddSellerCategory(categoryId, "Other Name");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.ErrorMessage.Should().Contain("already exists");
    }

    [Fact]
    public void AddSellerCategory_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid categoryId = Guid.Empty;
        string categoryName = "Electronics";

        // Act
        VoidResult result = seller.AddSellerCategory(categoryId, categoryName);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetSellerCategory_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string categoryName = "Electronics";
        seller.AddSellerCategory(categoryId, categoryName);
        
        SellerCategoryInfo addedCategory = seller.SellerCategories.First(sc => sc.CategoryId == categoryId);

        // Act
        Result<SellerCategoryInfo> result = seller.GetSellerCategory(addedCategory.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryId.Should().Be(categoryId);
        result.Value.Name.Should().Be(categoryName);
    }

    [Fact]
    public void GetSellerCategory_ShouldReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid nonExistentId = Guid.NewGuid();

        // Act
        Result<SellerCategoryInfo> result = seller.GetSellerCategory(nonExistentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void RemoveSellerCategory_ShouldRemoveCategory_WhenCategoryExists()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string categoryName = "Electronics";
        seller.AddSellerCategory(categoryId, categoryName);
        SellerCategoryInfo addedCategory = seller.SellerCategories.First(sc => sc.CategoryId == categoryId);

        // Act
        VoidResult result = seller.RemoveSellerCategory(addedCategory.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        seller.SellerCategories.Should().NotContain(sc => sc.Id == addedCategory.Id);
    }

    [Fact]
    public void RemoveSellerCategory_ShouldReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        Guid nonExistentId = Guid.NewGuid();

        // Act
        VoidResult result = seller.RemoveSellerCategory(nonExistentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void RemoveAllSellerCategories_ShouldClearCategories()
    {
        // Arrange
        SellerEntity seller = CreateSeller();
        seller.AddSellerCategory(Guid.NewGuid(), "Cat1");
        seller.AddSellerCategory(Guid.NewGuid(), "Cat2");

        // Act
        seller.RemoveAllSellerCategories();

        // Assert
        seller.SellerCategories.Should().BeEmpty();
    }

    private static SellerEntity CreateSeller()
    {
        return SellerEntity.Create(
            Guid.NewGuid(), 
            "John", "Doe", "Smith", 
            "+380123456789", 
            "Kyiv", "Street", 1, null, 
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))).Value!;
    }
}