using FluentAssertions;
using Order.Domain.OrderAggregate.Enums;
using Shared.Domain.Models;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Tests.Domain.Order;

public class OrderTests
{
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
    
    [Fact]
    public void Create_Should_ReturnOrder_When_AllParametersAreValid()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PaymentOptionIsWhileReceivingAndStatusIsPaid()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder(
            paymentOption: OrderPaymentOption.WhileReceiving,
            paymentStatus: OrderPaymentStatus.Paid);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Order cannot be paid already when payment option is while receiving");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CustomerIdIsEmpty()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder(customerId: Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("customerInfo cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_WithBonusesIsTrueAndUsedBonusesIsZero()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder(withBonuses: true, usedBonusesAmount: 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("usedBonusesAmount cannot be 0 when with bonuses is true");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_WithBonusesIsFalseAndUsedBonusesIsGreaterThanZero()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder(withBonuses: false, usedBonusesAmount: 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("usedBonusesAmount cannot be greater than 0 when with bonuses is false");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PriceIsZero()
    {
        // Arrange & Act
        Result<OrderAggregate> result = CreateOrder(price: 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("price must be positive");
    }

    [Fact]
    public void AddPaymentIntentId_Should_ReturnSuccess_When_PaymentIntentIdIsValid()
    {
        // Arrange
        OrderAggregate order = CreateOrder().Value!;
        var paymentIntentId = "pi_123456789";

        // Act
        VoidResult result = order.AddPaymentIntentId(paymentIntentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.PaymentIntentId.Should().Be(paymentIntentId);
        order.PaymentStatus.Should().Be(OrderPaymentStatus.Pending);
    }

    [Fact]
    public void AddPaymentIntentId_Should_ReturnFailure_When_PaymentIntentIdIsNullOrEmpty()
    {
        // Arrange
        OrderAggregate order = CreateOrder().Value!;

        // Act
        VoidResult result = order.AddPaymentIntentId(string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("paymentIntentId cannot be null or empty");
    }

    [Fact]
    public void MarkAsPaid_Should_ReturnSuccess_When_OrderCanBePaid()
    {
        // Arrange
        OrderAggregate order = CreateOrder().Value!;
        order.AddPaymentIntentId("pi_123456789");

        // Act
        VoidResult result = order.MarkAsPaid();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.PaymentStatus.Should().Be(OrderPaymentStatus.Paid);
    }

    [Fact]
    public void MarkAsPaid_Should_ReturnFailure_When_PaymentOptionIsWhileReceiving()
    {
        // Arrange
        OrderAggregate order = CreateOrder(paymentOption: OrderPaymentOption.WhileReceiving).Value!;

        // Act
        VoidResult result = order.MarkAsPaid();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Order cannot be paid when payment option is while receiving");
    }

    [Fact]
    public void AddOrderItem_Should_ReturnSuccess_When_OrderItemIsValid()
    {
        // Arrange
        OrderAggregate order = CreateOrder().Value!;
        var productId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var photoUrl = "http://example.com/photo.jpg";
        var title = "Product Title";
        uint quantity = 1;
        decimal price = 100;

        // Act
        VoidResult result = order.AddOrderItem(productId, sellerId, cartItemId, photoUrl, title, quantity, price);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.OrderItems.Should().HaveCount(1);
    }

    [Fact]
    public void AddOrderItem_Should_ReturnFailure_When_OrderItemWithSameProductExists()
    {
        // Arrange
        OrderAggregate order = CreateOrder().Value!;
        var productId = Guid.NewGuid();
        order.AddOrderItem(productId, Guid.NewGuid(), Guid.NewGuid(), "url", "title", 1, 100);

        // Act
        VoidResult result = order.AddOrderItem(productId, Guid.NewGuid(), Guid.NewGuid(), "url2", "title2", 2, 200);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be($"Order item with product id: {productId} already exist");
    }
}