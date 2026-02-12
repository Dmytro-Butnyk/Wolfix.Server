using System.Net;
using FluentAssertions;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryCreateTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "Electronics";
        var description = "Electronic devices";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Value!.BlobResourceId.Should().Be(blobResourceId);
        result.Value.PhotoUrl.Should().Be(photoUrl);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.ProductsCount.Should().Be(0);
        result.Value.IsParent.Should().BeTrue();
        result.Value.IsChild.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidDataAndParent_ShouldReturnSuccessResult()
    {
        // Arrange
        var parentBlobResourceId = Guid.NewGuid();
        var parentPhotoUrl = "https://example.com/parent.jpg";
        var parentName = "Electronics";
        var parentResult = CategoryAggregate.Create(parentBlobResourceId, parentPhotoUrl, parentName);
        
        var childBlobResourceId = Guid.NewGuid();
        var childPhotoUrl = "https://example.com/child.jpg";
        var childName = "Smartphones";

        // Act
        var result = CategoryAggregate.Create(childBlobResourceId, childPhotoUrl, childName, null, parentResult.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.Parent.Should().Be(parentResult.Value);
        result.Value.IsParent.Should().BeFalse();
        result.Value.IsChild.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_WithoutDescription_ShouldReturnSuccessResult()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "Electronics";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.Description.Should().BeNull();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyBlobResourceId_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.Empty;
        var photoUrl = "https://example.com/photo.jpg";
        var name = "Electronics";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("blobResourceId");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullPhotoUrl_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        string photoUrl = null!;
        var name = "Electronics";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyPhotoUrl_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "";
        var name = "Electronics";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithWhiteSpacePhotoUrl_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "   ";
        var name = "Electronics";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullName_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        string name = null!;

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceName_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "   ";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "Electronics";
        var description = "";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name, description);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceDescription_ShouldReturnFailure()
    {
        // Arrange
        var blobResourceId = Guid.NewGuid();
        var photoUrl = "https://example.com/photo.jpg";
        var name = "Electronics";
        var description = "   ";

        // Act
        var result = CategoryAggregate.Create(blobResourceId, photoUrl, name, description);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.Value.Should().BeNull();
    }
}