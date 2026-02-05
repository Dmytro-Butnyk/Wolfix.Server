
using System.Net;
using Catalog.Domain.CategoryAggregate;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryProductAttributesTests
{
    [Fact]
    public void AddProductAttribute_WithValidKey_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var key = "Brand";

        // Act
        var result = category.AddProductAttribute(key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().HaveCount(1);
        category.ProductAttributes.First().Key.Should().Be(key);
    }

    [Fact]
    public void AddProductAttribute_WithDuplicateKey_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var key = "Brand";
        category.AddProductAttribute(key);

        // Act
        var result = category.AddProductAttribute(key);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductAttribute");
        result.ErrorMessage.Should().Contain("already exists");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        category.ProductAttributes.Should().HaveCount(1);
    }
}

public class CategoryAddProductAttributesTests
{
    [Fact]
    public void AddProductAttributes_WithValidKeys_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var keys = new[] { "Brand", "Model", "Year" };

        // Act
        var result = category.AddProductAttributes(keys);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().HaveCount(3);
        category.ProductAttributes.Select(pa => pa.Key).Should().BeEquivalentTo(keys);
    }

    [Fact]
    public void AddProductAttributes_WithDuplicateKey_ShouldReturnFailureOnFirstDuplicate()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttribute("Brand");
        var keys = new[] { "Model", "Brand", "Year" };

        // Act
        var result = category.AddProductAttributes(keys);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductAttribute");
        result.ErrorMessage.Should().Contain("already exists");
        category.ProductAttributes.Should().HaveCount(2); // Original "Brand" + new "Model"
    }

    [Fact]
    public void AddProductAttributes_WithEmptyCollection_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var keys = Array.Empty<string>();

        // Act
        var result = category.AddProductAttributes(keys);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().BeEmpty();
    }
}

public class CategoryRemoveProductAttributeTests
{
    [Fact]
    public void RemoveProductAttribute_WithExistingId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttribute("Brand");
        var attributeId = category.ProductAttributes.First().Id;

        // Act
        var result = category.RemoveProductAttribute(attributeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().BeEmpty();
    }

    [Fact]
    public void RemoveProductAttribute_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = category.RemoveProductAttribute(nonExistingId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductAttribute");
        result.ErrorMessage.Should().Contain("does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void RemoveProductAttribute_RemovesCorrectAttribute_WhenMultipleExist()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttributes("Brand", "Model", "Year");
        var attributeToRemove = category.ProductAttributes.First(pa => pa.Key == "Model");

        // Act
        var result = category.RemoveProductAttribute(attributeToRemove.Key);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        category.ProductAttributes.Should().HaveCount(2);
        category.ProductAttributes.Should().NotContain(pa => pa.Key == attributeToRemove.Key);
        category.ProductAttributes.Select(pa => pa.Key).Should().BeEquivalentTo(new[] { "Brand", "Year" });
    }
}

public class CategoryRemoveAllProductAttributesTests
{
    [Fact]
    public void RemoveAllProductAttributes_WithAttributes_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttributes(new[] { "Brand", "Model", "Year" });

        // Act
        var result = category.RemoveAllProductAttributes();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().BeEmpty();
    }

    [Fact]
    public void RemoveAllProductAttributes_WithNoAttributes_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        var result = category.RemoveAllProductAttributes();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductAttributes.Should().BeEmpty();
    }
}

public class CategoryChangeProductAttributeKeyTests
{
    [Fact]
    public void ChangeProductAttributeKey_WithValidKey_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttribute("Brand");
        var attributeId = category.ProductAttributes.First().Id;
        var newKey = "Manufacturer";

        // Act
        var result = category.ChangeProductAttributeKey(attributeId, newKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var attribute = category.ProductAttributes.First(pa => pa.Id == attributeId);
        attribute.Key.Should().Be(newKey);
    }

    [Fact]
    public void ChangeProductAttributeKey_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();
        var newKey = "Brand";

        // Act
        var result = category.ChangeProductAttributeKey(nonExistingId, newKey);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("existingProductAttribute");
        result.ErrorMessage.Should().Contain("does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class CategoryGetProductAttributeTests
{
    [Fact]
    public void GetProductAttribute_WithExistingId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttribute("Brand");
        var attributeId = category.ProductAttributes.First().Id;

        // Act
        var result = category.GetProductAttribute(attributeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(attributeId);
        result.Value.Key.Should().Be("Brand");
    }

    [Fact]
    public void GetProductAttribute_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = category.GetProductAttribute(nonExistingId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("productAttribute");
        result.ErrorMessage.Should().Contain("is null");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
    }
}