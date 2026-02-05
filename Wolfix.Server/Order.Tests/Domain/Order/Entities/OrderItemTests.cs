using FluentAssertions;
using Order.Domain.OrderAggregate.Entities;
using Order.Domain.OrderAggregate.Enums;
using Shared.Domain.Models;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Tests.Domain.Order.Entities;

public class OrderItemTests
{
    [Fact]
    public void Create_Should_ReturnOrderItem_When_AllParametersAreValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Product Title";
        uint quantity = 1;
        decimal price = 100;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(productId, sellerId, cartItemId, photoUrl, title, quantity, price, order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ProductIdIsEmpty()
    {
        // Arrange
        var productId = Guid.Empty;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(productId, Guid.NewGuid(), Guid.NewGuid(), "url", "title", 1, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("productId cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_SellerIdIsEmpty()
    {
        // Arrange
        var sellerId = Guid.Empty;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), sellerId, Guid.NewGuid(), "url", "title", 1, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("sellerId cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CartItemIdIsEmpty()
    {
        // Arrange
        var cartItemId = Guid.Empty;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), cartItemId, "url", "title", 1, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("cartItemId cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PhotoUrlIsNullOrEmpty()
    {
        // Arrange
        var photoUrl = string.Empty;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), photoUrl, "title", 1, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("photoUrl cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_TitleIsNullOrEmpty()
    {
        // Arrange
        var title = string.Empty;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "url", title, 1, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("title cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_QuantityIsZero()
    {
        // Arrange
        uint quantity = 0;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "url", "title", quantity, 100, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("quantity must be positive");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PriceIsZero()
    {
        // Arrange
        decimal price = 0;
        Result<OrderAggregate> orderResult = CreateOrder();
        OrderAggregate order = orderResult.Value!;

        // Act
        Result<OrderItem> result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "url", "title", 1, price, order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("price must be positive");
    }

    private Result<OrderAggregate> CreateOrder(
        string customerFirstName = "John",
        string customerLastName = "Doe",
        string customerMiddleName = "Middle",
        string customerPhoneNumber = "+380871235825",
        string customerEmail = "john.doe@example.com",
        Guid? customerId = null,
        string recipientFirstName = "Jane",
        string recipientLastName = "Doe",
        string recipientMiddleName = "Middle",
        string recipientPhoneNumber = "+380871235825",
        OrderPaymentOption paymentOption = OrderPaymentOption.Card,
        OrderPaymentStatus paymentStatus = OrderPaymentStatus.Pending,
        string deliveryMethodName = "Courier",
        uint? deliveryInfoNumber = 123,
        string deliveryInfoCity = "City",
        string deliveryInfoStreet = "Street",
        uint deliveryInfoHouseNumber = 45,
        DeliveryOption deliveryOption = DeliveryOption.Courier,
        bool withBonuses = false,
        decimal usedBonusesAmount = 0,
        decimal price = 100)
        => OrderAggregate.Create(
            customerFirstName,
            customerLastName,
            customerMiddleName,
            customerPhoneNumber,
            customerEmail,
            customerId ?? Guid.NewGuid(),
            recipientFirstName,
            recipientLastName,
            recipientMiddleName,
            recipientPhoneNumber,
            paymentOption,
            paymentStatus,
            deliveryMethodName,
            deliveryInfoNumber,
            deliveryInfoCity,
            deliveryInfoStreet,
            deliveryInfoHouseNumber,
            deliveryOption,
            withBonuses,
            usedBonusesAmount,
            price);
}