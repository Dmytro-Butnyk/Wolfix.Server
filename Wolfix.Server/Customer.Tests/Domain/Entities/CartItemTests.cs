using Customer.Domain.CustomerAggregate.Entities;
using FluentAssertions;
using Shared.Domain.Models;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Tests.Domain.Entities;

public class CartItemTests
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
        decimal priceWithDiscount = 100;
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Customer.Should().Be(customer);
        result.Value.PhotoUrl.Should().Be(photoUrl);
        result.Value.Title.Should().Be(title);
        result.Value.PriceWithDiscount.Should().Be(priceWithDiscount);
        result.Value.ProductId.Should().Be(productId);
        result.Value.SellerId.Should().Be(sellerId);
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenProductIdIsEmpty()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Test Item";
        decimal priceWithDiscount = 100;
        var productId = Guid.Empty;
        var sellerId = Guid.NewGuid();

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("productId cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenPhotoUrlIsEmpty()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "";
        var title = "Test Item";
        decimal priceWithDiscount = 100;
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

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
        decimal priceWithDiscount = 100;
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

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
        decimal priceWithDiscount = 0;
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("priceWithDiscount cannot be less than or equal to zero");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_WhenSellerIdIsEmpty()
    {
        // Arrange
        CustomerAggregate customer = CreateCustomer();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Test Item";
        decimal priceWithDiscount = 100;
        var productId = Guid.NewGuid();
        var sellerId = Guid.Empty;

        // Act
        Result<CartItem> result = CartItem.Create(customer, photoUrl, title, priceWithDiscount, productId, sellerId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("sellerId cannot be empty");
    }
}