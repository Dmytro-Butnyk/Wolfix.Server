using System.Net;
using Catalog.Domain.CategoryAggregate;
using FluentAssertions;
using Xunit;
using CategoryAggregate = Catalog.Domain.CategoryAggregate.Category;

namespace Catalog.Tests.Domain.Category;

public class CategoryChangeMethodsTests
{
    [Fact]
    public void ChangePhotoUrl_WithValidUrl_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/old.jpg", "Electronics").Value!;
        var newPhotoUrl = "https://example.com/new.jpg";

        // Act
        var result = category.ChangePhotoUrl(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.PhotoUrl.Should().Be(newPhotoUrl);
    }

    [Fact]
    public void ChangePhotoUrl_WithNullUrl_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/old.jpg", "Electronics").Value!;
        string newPhotoUrl = null!;

        // Act
        var result = category.ChangePhotoUrl(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangePhotoUrl_WithEmptyUrl_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/old.jpg", "Electronics").Value!;
        var newPhotoUrl = "";

        // Act
        var result = category.ChangePhotoUrl(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangePhotoUrl_WithWhiteSpaceUrl_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/old.jpg", "Electronics").Value!;
        var newPhotoUrl = "   ";

        // Act
        var result = category.ChangePhotoUrl(newPhotoUrl);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("photoUrl");
        result.ErrorMessage.Should().Contain("cannot be empty");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public class CategoryChangeNameTests
{
    [Fact]
    public void ChangeName_WithValidName_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var newName = "Home Electronics";

        // Act
        var result = category.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.Name.Should().Be(newName);
    }

    [Fact]
    public void ChangeName_WithSameName_ShouldReturnFailure()
    {
        // Arrange
        var existingName = "Electronics";
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", existingName).Value!;

        // Act
        var result = category.ChangeName(existingName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("name");
        result.ErrorMessage.Should().Contain("the same as the current one");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangeName_WithNullName_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        string newName = null!;

        // Act
        var result = category.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangeName_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var newName = "";

        // Act
        var result = category.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangeName_WithWhiteSpaceName_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var newName = "   ";

        // Act
        var result = category.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("text");
        result.ErrorMessage.Should().Contain("is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public class CategoryChangeDescriptionTests
{
    [Fact]
    public void ChangeDescription_WithValidDescription_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics", "Old description").Value!;
        var newDescription = "New description";

        // Act
        var result = category.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.Description.Should().Be(newDescription);
    }

    [Fact]
    public void ChangeDescription_ToNull_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics", "Old description").Value!;
        string? newDescription = null;

        // Act
        var result = category.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.Description.Should().BeNull();
    }

    [Fact]
    public void ChangeDescription_FromNullToValue_ShouldReturnSuccess()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics").Value!;
        var newDescription = "New description";

        // Act
        var result = category.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        category.Description.Should().Be(newDescription);
    }

    [Fact]
    public void ChangeDescription_WithSameDescription_ShouldReturnFailure()
    {
        // Arrange
        var existingDescription = "Electronics description";
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics", existingDescription).Value!;

        // Act
        var result = category.ChangeDescription(existingDescription);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("description");
        result.ErrorMessage.Should().Contain("the same as the current one");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangeDescription_WithEmptyString_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics", "Old description").Value!;
        var newDescription = "";

        // Act
        var result = category.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("description");
        result.ErrorMessage.Should().Contain("cannot be empty or white space");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void ChangeDescription_WithWhiteSpace_ShouldReturnFailure()
    {
        // Arrange
        var category = CategoryAggregate.Create(Guid.NewGuid(), "https://example.com/photo.jpg", "Electronics", "Old description").Value!;
        var newDescription = " ";

        // Act
        var result = category.ChangeDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("description");
        result.ErrorMessage.Should().Contain("cannot be empty or white space");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}