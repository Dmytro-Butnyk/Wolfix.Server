using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;
using ProductVariantEntity = Catalog.Domain.CategoryAggregate.Entities.ProductVariant;


namespace Catalog.Tests.Domain.Category.Entities.ProductVariant;

public class ProductVariantTests
{
    private readonly CategoryAggregate _category = CategoryAggregate.Create(Guid.NewGuid(), "http://photo.url", "Electronics").Value!;
    
    [Fact]
    public void Create_Should_ReturnSuccess_WhenKeyIsValid()
    {
        // Arrange
        const string key = "Color";

        // Act
        var result = ProductVariantEntity.Create(_category, key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Key.Should().Be(key);
        result.Value.Category.Should().Be(_category);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ReturnFailure_WhenKeyIsInvalid(string key)
    {
        // Act
        var result = ProductVariantEntity.Create(_category, key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetKey_Should_UpdateKey_WhenKeyIsValid()
    {
        // Arrange
        var variant = ProductVariantEntity.Create(_category, "Size").Value!;
        const string newKey = "Weight";

        // Act
        var result = variant.SetKey(newKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        variant.Key.Should().Be(newKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetKey_Should_ReturnFailure_WhenKeyIsInvalid(string key)
    {
        // Arrange
        var variant = ProductVariantEntity.Create(_category, "Size").Value!;

        // Act
        var result = variant.SetKey(key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExplicitOperator_Should_ConvertToProductVariantInfo()
    {
        // Arrange
        var variant = ProductVariantEntity.Create(_category, "Material").Value!;

        // Act
        var info = (ProductVariantInfo)variant;

        // Assert
        info.Should().NotBeNull();
        info.Id.Should().Be(variant.Id);
        info.Key.Should().Be(variant.Key);
    }
}