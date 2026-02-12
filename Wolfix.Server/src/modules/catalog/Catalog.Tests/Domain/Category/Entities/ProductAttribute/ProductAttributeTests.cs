using Catalog.Domain.CategoryAggregate.Entities;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;
using ProductAttributeEntity = Catalog.Domain.CategoryAggregate.Entities.ProductAttribute;

namespace Catalog.Tests.Domain.Category.Entities.ProductAttribute;

public class ProductAttributeTests
{
    private readonly CategoryAggregate _category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
    
    [Fact]
    public void Create_Should_ReturnSuccess_WhenKeyIsValid()
    {
        // Arrange
        var key = "Color";

        // Act
        var result = ProductAttributeEntity.Create(_category, key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Key.Should().Be(key);
        result.Value!.Category.Should().Be(_category);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ReturnFailure_WhenKeyIsInvalid(string key)
    {
        // Act
        var result = ProductAttributeEntity.Create(_category, key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetKey_Should_UpdateKey_WhenKeyIsValid()
    {
        // Arrange
        var attribute = ProductAttributeEntity.Create(_category, "Size").Value!;
        var newKey = "Weight";

        // Act
        var result = attribute.SetKey(newKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.Key.Should().Be(newKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetKey_Should_ReturnFailure_WhenKeyIsInvalid(string key)
    {
        // Arrange
        var attribute = ProductAttributeEntity.Create(_category, "Size").Value!;

        // Act
        var result = attribute.SetKey(key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExplicitOperator_Should_ConvertToProductAttributeInfo()
    {
        // Arrange
        var attribute = ProductAttributeEntity.Create(_category, "Material").Value!;

        // Act
        var info = (ProductAttributeInfo)attribute;

        // Assert
        info.Should().NotBeNull();
        info.Id.Should().Be(attribute.Id);
        info.Key.Should().Be(attribute.Key);
    }
}