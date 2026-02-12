using System;
using System.Net;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Domain.ProductAggregate.Enums;
using FluentAssertions;
using Xunit;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product.Entities;

public class DiscountTests
{
    // Helper method to create a valid Product for testing
    private ProductAggregate CreateValidProduct()
    {
        var title = "Test Product";
        var description = "Test Description";
        var price = 100.0m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        return ProductAggregate.Create(title, description, price, status, categoryId, sellerId).Value!;
    }

    [Fact]
    public void Create_ValidParameters_ReturnsSuccess()
    {
        // Arrange
        uint percent = 10;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();

        // Act
        Shared.Domain.Models.Result<Discount> result = Discount.Create(percent, expirationDateTime, product);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Percent.Should().Be(percent);
        result.Value.ExpirationDateTime.Should().Be(expirationDateTime);
        result.Value.Product.Should().Be(product);
        result.Value.ProductId.Should().Be(product.Id);
        result.Value.Status.Should().Be(DiscountStatus.Active);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData(0, "percent must be positive")]
    [InlineData(101, "percent must be less than 100")]
    public void Create_InvalidPercent_ReturnsFailure(uint percent, string expectedErrorMessage)
    {
        // Arrange
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();

        // Act
        Shared.Domain.Models.Result<Discount> result = Discount.Create(percent, expirationDateTime, product);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be(expectedErrorMessage);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_InvalidExpirationDateTime_ReturnsFailure()
    {
        // Arrange
        uint percent = 10;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(-1); // Past date
        ProductAggregate product = CreateValidProduct();

        // Act
        Shared.Domain.Models.Result<Discount> result = Discount.Create(percent, expirationDateTime, product);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("expirationDateTime must be greater than now");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void SetStatus_ValidStatus_ReturnsSuccess()
    {
        // Arrange
        uint percent = 10;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(percent, expirationDateTime, product).Value!;
        var newStatus = DiscountStatus.Expired;

        // Act
        Shared.Domain.Models.VoidResult result = discount.SetStatus(newStatus);

        // Assert
        result.IsSuccess.Should().BeTrue();
        discount.Status.Should().Be(newStatus);
    }

    [Fact]
    public void SetPercent_ValidPercent_ReturnsSuccess()
    {
        // Arrange
        uint initialPercent = 10;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(initialPercent, expirationDateTime, product).Value!;
        uint newPercent = 20;

        // Act
        Shared.Domain.Models.VoidResult result = discount.SetPercent(newPercent);

        // Assert
        result.IsSuccess.Should().BeTrue();
        discount.Percent.Should().Be(newPercent);
    }

    [Theory]
    [InlineData(0, "percent must be positive")]
    [InlineData(101, "percent must be less than 100")]
    public void SetPercent_InvalidPercent_ReturnsFailure(uint newPercent, string expectedErrorMessage)
    {
        // Arrange
        uint initialPercent = 10;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(initialPercent, expirationDateTime, product).Value!;

        // Act
        Shared.Domain.Models.VoidResult result = discount.SetPercent(newPercent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be(expectedErrorMessage);
        discount.Percent.Should().Be(initialPercent); // Should not change
    }

    [Fact]
    public void SetExpirationDateTime_ValidDateTime_ReturnsSuccess()
    {
        // Arrange
        uint percent = 10;
        DateTime initialExpirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(percent, initialExpirationDateTime, product).Value!;
        DateTime newExpirationDateTime = DateTime.UtcNow.AddDays(14);

        // Act
        Shared.Domain.Models.VoidResult result = discount.SetExpirationDateTime(newExpirationDateTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        discount.ExpirationDateTime.Should().Be(newExpirationDateTime);
    }

    [Fact]
    public void SetExpirationDateTime_InvalidDateTime_ReturnsFailure()
    {
        // Arrange
        uint percent = 10;
        DateTime initialExpirationDateTime = DateTime.UtcNow.AddDays(7);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(percent, initialExpirationDateTime, product).Value!;
        DateTime newExpirationDateTime = DateTime.UtcNow.AddDays(-1); // Past date

        // Act
        Shared.Domain.Models.VoidResult result = discount.SetExpirationDateTime(newExpirationDateTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("expirationDateTime must be greater than now");
        discount.ExpirationDateTime.Should().Be(initialExpirationDateTime); // Should not change
    }

    [Fact]
    public void ExplicitOperator_DiscountToDiscountInfo_MapsCorrectly()
    {
        // Arrange
        uint percent = 15;
        DateTime expirationDateTime = DateTime.UtcNow.AddDays(10);
        ProductAggregate product = CreateValidProduct();
        Discount discount = Discount.Create(percent, expirationDateTime, product).Value!;

        // Act
        var discountInfo = (DiscountInfo)discount;

        // Assert
        discountInfo.Should().NotBeNull();
        discountInfo.Id.Should().Be(discount.Id);
        discountInfo.Percent.Should().Be(discount.Percent);
        discountInfo.ExpirationDateTime.Should().Be(discount.ExpirationDateTime);
        discountInfo.Status.Should().Be(discount.Status);
    }
}