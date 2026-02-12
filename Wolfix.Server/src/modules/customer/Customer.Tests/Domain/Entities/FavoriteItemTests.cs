using Customer.Domain.CustomerAggregate.Entities;
using FluentAssertions;
using Shared.Domain.Models;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Tests.Domain.Entities;

public class FavoriteItemTests
{
    private CustomerAggregate CreateCustomer()
        => CustomerAggregate.Create(Guid.NewGuid()).Value!;

    [Fact]
    public void Create_Should_ReturnSuccessResult_WhenParametersAreValid()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Test Item";
        decimal price = 100;
        uint bonuses = 10;

        // Act
        Result<FavoriteItem> result = FavoriteItem.Create(customer, photoUrl, title, price, bonuses);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Customer.Should().Be(customer);
        result.Value.PhotoUrl.Should().Be(photoUrl);
        result.Value.Title.Should().Be(title);
        result.Value.Price.Should().Be(price);
        result.Value.Bonuses.Should().Be(bonuses);
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenPhotoUrlIsEmpty()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "";
        var title = "Test Item";
        decimal price = 100;
        uint bonuses = 10;

        // Act
        Result<FavoriteItem> result = FavoriteItem.Create(customer, photoUrl, title, price, bonuses);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("photoUrl cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenTitleIsEmpty()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "";
        decimal price = 100;
        uint bonuses = 10;

        // Act
        Result<FavoriteItem> result = FavoriteItem.Create(customer, photoUrl, title, price, bonuses);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("title cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenPriceIsZero()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Test Item";
        decimal price = 0;
        uint bonuses = 10;

        // Act
        Result<FavoriteItem> result = FavoriteItem.Create(customer, photoUrl, title, price, bonuses);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("price cannot be less than or equal to zero");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenBonusesIsZero()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Test Item";
        decimal price = 100;
        uint bonuses = 0;

        // Act
        Result<FavoriteItem> result = FavoriteItem.Create(customer, photoUrl, title, price, bonuses);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("bonuses cannot be less than or equal to zero");
    }
}