using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryPropertiesTests
{
    [Fact]
    public void IsParent_WhenCategoryHasNoParent_ShouldReturnTrue()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        var isParent = category.IsParent;

        // Assert
        isParent.Should().BeTrue();
        category.Parent.Should().BeNull();
    }

    [Fact]
    public void IsParent_WhenCategoryHasParent_ShouldReturnFalse()
    {
        // Arrange
        var parent = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/parent.jpg", "Electronics").Value!;
        var child = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/child.jpg", "Smartphones", null, parent).Value!;

        // Act
        var isParent = child.IsParent;

        // Assert
        isParent.Should().BeFalse();
        child.Parent.Should().Be(parent);
    }
}

public class CategoryIsChildPropertyTests
{
    [Fact]
    public void IsChild_WhenCategoryHasParent_ShouldReturnTrue()
    {
        // Arrange
        var parent = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/parent.jpg", "Electronics").Value!;
        var child = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/child.jpg", "Smartphones", null, parent).Value!;

        // Act
        var isChild = child.IsChild;

        // Assert
        isChild.Should().BeTrue();
        child.Parent.Should().Be(parent);
    }

    [Fact]
    public void IsChild_WhenCategoryHasNoParent_ShouldReturnFalse()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        var isChild = category.IsChild;

        // Assert
        isChild.Should().BeFalse();
        category.Parent.Should().BeNull();
    }
}

public class CategoryProductsCountPropertyTests
{
    [Fact]
    public void ProductsCount_OnNewCategory_ShouldBeZero()
    {
        // Arrange & Act
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Assert
        category.ProductsCount.Should().Be(0);
    }

    [Fact]
    public void ProductsCount_AfterAddingProducts_ShouldBeUpdated()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;

        // Act
        category.AddProductId(Guid.NewGuid());
        category.AddProductId(Guid.NewGuid());
        category.AddProductId(Guid.NewGuid());

        // Assert
        category.ProductsCount.Should().Be(3);
    }

    [Fact]
    public void ProductsCount_AfterRemovingProduct_ShouldBeUpdated()
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
        category.RemoveProductId(productId2);

        // Assert
        category.ProductsCount.Should().Be(2);
    }

    [Fact]
    public void ProductsCount_AfterRemovingAllProducts_ShouldBeZero()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductId(Guid.NewGuid());
        category.AddProductId(Guid.NewGuid());
        category.AddProductId(Guid.NewGuid());

        // Act
        category.RemoveAllProductIds();

        // Assert
        category.ProductsCount.Should().Be(0);
    }
}

public class CategoryReadOnlyCollectionsTests
{
    [Fact]
    public void ProductIds_ShouldBeReadOnly()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductId(Guid.NewGuid());

        // Act
        var productIds = category.ProductIds;

        // Assert
        productIds.Should().BeAssignableTo<IReadOnlyCollection<Guid>>();
        productIds.Should().HaveCount(1);
    }

    [Fact]
    public void ProductVariants_ShouldBeReadOnly()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductVariant("Color");

        // Act
        var variants = category.ProductVariants;

        // Assert
        variants.Should().BeAssignableTo<IReadOnlyCollection<ProductVariantInfo>>();
        variants.Should().HaveCount(1);
    }

    [Fact]
    public void ProductAttributes_ShouldBeReadOnly()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        category.AddProductAttribute("Brand");

        // Act
        var attributes = category.ProductAttributes;

        // Assert
        attributes.Should().BeAssignableTo<IReadOnlyCollection<ProductAttributeInfo>>();
        attributes.Should().HaveCount(1);
    }
}

public class CategoryResultPropertiesTests
{
    [Fact]
    public void SuccessResult_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var result = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void FailureResult_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var result = CategoryAggregate.Create(Guid.Empty, "https://example.com/photo.jpg", "Electronics");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.Value.Should().BeNull();
    }
}