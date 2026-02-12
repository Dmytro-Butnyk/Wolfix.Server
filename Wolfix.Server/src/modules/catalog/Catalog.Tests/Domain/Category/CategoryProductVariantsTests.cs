
using System.Net;
using Catalog.Domain.CategoryAggregate;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryProductVariantsTests
{
    [Fact]
    public void AddProductVariant_WithValidKey_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var key = "Color";

        // Act
        var result = category.AddProductVariant(key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().HaveCount(1);
        category.ProductVariants.First().Key.Should().Be(key);
    }

    [Fact]
    public void AddProductVariant_WithDuplicateKey_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var key = "Color";
        category.AddProductVariant(key);

        // Act
        var result = category.AddProductVariant(key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductVariant");
        result.ErrorMessage.Should().Contain("already exists");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        category.ProductVariants.Should().HaveCount(1);
    }
}

public class CategoryAddProductVariantsTests
{
    [Fact]
    public void AddProductVariants_WithValidKeys_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var keys = new[] { "Color", "Size", "Material" };

        // Act
        var result = category.AddProductVariants(keys);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().HaveCount(3);
        category.ProductVariants.Select(pv => pv.Key).Should().BeEquivalentTo(keys);
    }

    [Fact]
    public void AddProductVariants_WithDuplicateKey_ShouldReturnFailureOnFirstDuplicate()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariant("Color");
        var keys = new[] { "Size", "Color", "Material" };

        // Act
        var result = category.AddProductVariants(keys);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductVariant");
        result.ErrorMessage.Should().Contain("already exists");
        category.ProductVariants.Should().HaveCount(2); // Original "Color" + new "Size"
    }

    [Fact]
    public void AddProductVariants_WithEmptyCollection_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var keys = Array.Empty<string>();

        // Act
        var result = category.AddProductVariants(keys);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().BeEmpty();
    }
}

public class CategoryRemoveProductVariantTests
{
    [Fact]
    public void RemoveProductVariant_WithExistingId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariant("Color");
        var variantId = category.ProductVariants.First().Id;

        // Act
        var result = category.RemoveProductVariant(variantId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().BeEmpty();
    }

    [Fact]
    public void RemoveProductVariant_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = category.RemoveProductVariant(nonExistingId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductVariant");
        result.ErrorMessage.Should().Contain("does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void RemoveProductVariant_RemovesCorrectVariant_WhenMultipleExist()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariants("Color", "Size", "Material");
        var variantToRemove = category.ProductVariants.First(pv => pv.Key == "Size");

        // Act
        var result = category.RemoveProductVariant(variantToRemove.Key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        category.ProductVariants.Should().HaveCount(2);
        category.ProductVariants.Should().NotContain(pv => pv.Key == variantToRemove.Key);
        category.ProductVariants.Select(pv => pv.Key).Should().BeEquivalentTo(new[] { "Color", "Material" });
    }
}

public class CategoryRemoveAllProductVariantsTests
{
    [Fact]
    public void RemoveAllProductVariants_WithVariants_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariants(new[] { "Color", "Size", "Material" });

        // Act
        var result = category.RemoveAllProductVariants();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().BeEmpty();
    }

    [Fact]
    public void RemoveAllProductVariants_WithNoVariants_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        var result = category.RemoveAllProductVariants();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductVariants.Should().BeEmpty();
    }
}

public class CategoryChangeProductVariantKeyTests
{
    [Fact]
    public void ChangeProductVariantKey_WithValidKey_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariant("Color");
        var variantId = category.ProductVariants.First().Id;
        var newKey = "Colour";

        // Act
        var result = category.ChangeProductVariantKey(variantId, newKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var variant = category.ProductVariants.First(pv => pv.Id == variantId);
        variant.Key.Should().Be(newKey);
    }

    [Fact]
    public void ChangeProductVariantKey_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();
        var newKey = "Color";

        // Act
        var result = category.ChangeProductVariantKey(nonExistingId, newKey);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductVariant");
        result.ErrorMessage.Should().Contain("does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class CategoryGetProductVariantTests
{
    [Fact]
    public void GetProductVariant_WithExistingId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariant("Color");
        var variantId = category.ProductVariants.First().Id;

        // Act
        var result = category.GetProductVariant(variantId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(variantId);
        result.Value.Key.Should().Be("Color");
    }

    [Fact]
    public void GetProductVariant_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = category.GetProductVariant(nonExistingId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("productVariant");
        result.ErrorMessage.Should().Contain("is null");
        result.Value.Should().BeNull();
    }
}