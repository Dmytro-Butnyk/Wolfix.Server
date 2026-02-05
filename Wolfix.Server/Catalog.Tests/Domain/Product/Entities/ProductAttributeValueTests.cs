using Catalog.Domain.ProductAggregate.Entities;
using FluentAssertions;
using Catalog.Domain.ProductAggregate.Enums;
using Shared.Domain.Models;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product.Entities;

public class ProductAttributeValueTests
{
    private static ProductAggregate CreateProduct()
    {
        var name = "Test Product";
        var description = "Test Description";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var result = ProductAggregate.Create(name, description, price, status, categoryId, brandId);
        if (!result.IsSuccess)
        {
            throw new Exception("Failed to create product for testing");
        }
        return result.Value!;
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidParameters()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        var key = "Color";
        var value = "Red";
        var categoryAttributeId = Guid.NewGuid();

        // Act
        Result<ProductAttributeValue> result = ProductAttributeValue.Create(product, key, value, categoryAttributeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Product.Should().Be(product);
        result.Value.Key.Should().Be(key);
        result.Value.Value.Should().Be(value);
        result.Value.CategoryAttributeId.Should().Be(categoryAttributeId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ReturnFailure_When_KeyIsInvalid(string invalidKey)
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        string value = "Red";
        Guid categoryAttributeId = Guid.NewGuid();

        // Act
        Result<ProductAttributeValue> result = ProductAttributeValue.Create(product, invalidKey, value, categoryAttributeId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CategoryAttributeIdIsEmpty()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        string key = "Color";
        string value = "Red";
        Guid categoryAttributeId = Guid.Empty;

        // Act
        Result<ProductAttributeValue> result = ProductAttributeValue.Create(product, key, value, categoryAttributeId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be($"{nameof(categoryAttributeId)} is required");
    }

    [Fact]
    public void SetValue_Should_UpdateValue_When_NewValueIsValid()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        Result<ProductAttributeValue> attribute = ProductAttributeValue.Create(product, "Color", "Red", Guid.NewGuid());
        string newValue = "Blue";

        // Act
        VoidResult result = attribute.Value.SetValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.Value.Value.Should().Be(newValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetValue_Should_ReturnFailure_When_NewValueIsInvalid(string invalidValue)
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        Result<ProductAttributeValue> attribute = ProductAttributeValue.Create(product, "Color", "Red", Guid.NewGuid());

        // Act
        VoidResult result = attribute.Value.SetValue(invalidValue);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetKey_Should_UpdateKey_When_NewKeyIsValid()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        Result<ProductAttributeValue> attribute = ProductAttributeValue.Create(product, "Color", "Red", Guid.NewGuid());
        string newKey = "Material";

        // Act
        VoidResult result = attribute.Value.SetKey(newKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.Value.Key.Should().Be(newKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetKey_Should_ReturnFailure_When_NewKeyIsInvalid(string invalidKey)
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        Result<ProductAttributeValue> attribute = ProductAttributeValue.Create(product, "Color", "Red", Guid.NewGuid());

        // Act
        VoidResult result = attribute.Value.SetKey(invalidKey);



        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExplicitOperator_Should_ReturnCorrectProductAttributeValueInfo()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        string key = "Size";
        string value = "XL";
        Guid categoryAttributeId = Guid.NewGuid();
        Result<ProductAttributeValue> attributeResult = ProductAttributeValue.Create(product, key, value, categoryAttributeId);
        ProductAttributeValue attribute = attributeResult.Value;

        // Act
        ProductAttributeValueInfo info = (ProductAttributeValueInfo)attribute;

        // Assert
        info.Should().NotBeNull();
        info.Id.Should().Be(attribute.Id);
        info.Key.Should().Be(key);
        info.Value.Should().Be(value);
        info.CategoryAttributeId.Should().Be(categoryAttributeId);
    }
}