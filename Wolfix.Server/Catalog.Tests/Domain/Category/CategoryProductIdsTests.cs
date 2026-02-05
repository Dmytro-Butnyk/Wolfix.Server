using System.Net;
using Catalog.Domain.CategoryAggregate;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryProductIdsTests
{
    [Fact]
    public void AddProductId_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.NewGuid();

        // Act
        var result = category.AddProductId(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductIds.Should().Contain(productId);
        category.ProductsCount.Should().Be(1);
    }

    [Fact]
    public void AddProductId_MultipleProducts_ShouldUpdateCount()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();

        // Act
        category.AddProductId(productId1);
        category.AddProductId(productId2);
        var result = category.AddProductId(productId3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        category.ProductIds.Should().HaveCount(3);
        category.ProductsCount.Should().Be(3);
        category.ProductIds.Should().Contain(new[] { productId1, productId2, productId3 });
    }

    [Fact]
    public void AddProductId_WithEmptyGuid_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.Empty;

        // Act
        var result = category.AddProductId(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("guid");
        result.ErrorMessage.Should().Contain("is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        category.ProductIds.Should().BeEmpty();
        category.ProductsCount.Should().Be(0);
    }

    [Fact]
    public void AddProductId_WithDuplicateId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.NewGuid();
        category.AddProductId(productId);

        // Act
        var result = category.AddProductId(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("productId");
        result.ErrorMessage.Should().Contain("already exists");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        category.ProductIds.Should().HaveCount(1);
        category.ProductsCount.Should().Be(1);
    }
}

public class CategoryRemoveProductIdTests
{
    [Fact]
    public void RemoveProductId_WithExistingId_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.NewGuid();
        category.AddProductId(productId);

        // Act
        var result = category.RemoveProductId(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductIds.Should().NotContain(productId);
        category.ProductsCount.Should().Be(0);
    }

    [Fact]
    public void RemoveProductId_MultipleProducts_ShouldUpdateCount()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();
        category.AddProductId(productId1);
        category.AddProductId(productId2);
        category.AddProductId(productId3);

        // Act
        var result = category.RemoveProductId(productId2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        category.ProductIds.Should().HaveCount(2);
        category.ProductsCount.Should().Be(2);
        category.ProductIds.Should().Contain(new[] { productId1, productId3 });
        category.ProductIds.Should().NotContain(productId2);
    }

    [Fact]
    public void RemoveProductId_WithEmptyGuid_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.Empty;

        // Act
        var result = category.RemoveProductId(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("guid");
        result.ErrorMessage.Should().Contain("is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void RemoveProductId_WithNonExistingId_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId = Guid.NewGuid();

        // Act
        var result = category.RemoveProductId(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("productId");
        result.ErrorMessage.Should().Contain("does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}

public class CategoryRemoveAllProductIdsTests
{
    [Fact]
    public void RemoveAllProductIds_WithProducts_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var productId3 = Guid.NewGuid();
        category.AddProductId(productId1);
        category.AddProductId(productId2);
        category.AddProductId(productId3);

        // Act
        var result = category.RemoveAllProductIds();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductIds.Should().BeEmpty();
        category.ProductsCount.Should().Be(0);
    }

    [Fact]
    public void RemoveAllProductIds_WithNoProducts_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        var result = category.RemoveAllProductIds();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.ProductIds.Should().BeEmpty();
        category.ProductsCount.Should().Be(0);
    }
}