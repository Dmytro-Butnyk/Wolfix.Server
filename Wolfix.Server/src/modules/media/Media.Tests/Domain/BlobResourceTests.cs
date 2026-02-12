using Media.Domain.BlobAggregate;
using Shared.Domain.Enums;
using FluentAssertions;
using Shared.Domain.Models;
using Xunit;

namespace Media.Tests.Domain;

public class BlobResourceTests
{
    [Fact]
    public void Create_ShouldReturnSuccessResult_WhenTypeIsDefined()
    {
        // Arrange
        const string type = nameof(BlobResourceType.Photo);

        // Act
        Result<BlobResource> result = BlobResource.Create(type);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Type.Should().Be(BlobResourceType.Photo);
        result.Value.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Create_ShouldReturnFailureResult_WhenTypeIsNotDefined()
    {
        // Arrange
        var type = "fjhgjdflkgh";

        // Act
        Result<BlobResource> result = BlobResource.Create(type);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid blob resource type");
    }

    [Fact]
    public void ChangeName_ShouldReturnSuccessResult_WhenNameIsValid()
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;
        var newName = "newName";

        // Act
        VoidResult result = blobResource.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        blobResource.Name.Should().Be(newName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeName_ShouldReturnFailureResult_WhenNameIsInvalid(string newName)
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;

        // Act
        VoidResult result = blobResource.ChangeName(newName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("text is required");
    }

    [Fact]
    public void ChangeUrl_ShouldReturnSuccessResult_WhenUrlIsValid()
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;
        var newUrl = "newUrl";

        // Act
        VoidResult result = blobResource.ChangeUrl(newUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        blobResource.Url.Should().Be(newUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeUrl_ShouldReturnFailureResult_WhenUrlIsInvalid(string newUrl)
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;

        // Act
        VoidResult result = blobResource.ChangeUrl(newUrl);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("text is required");
    }

    [Fact]
    public void ChangeType_ShouldReturnSuccessResult_WhenTypeIsDefined()
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;
        string newType = nameof(BlobResourceType.Video);

        // Act
        VoidResult result = blobResource.ChangeType(newType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        blobResource.Type.Should().Be(BlobResourceType.Video);
    }

    [Fact]
    public void ChangeType_ShouldReturnFailureResult_WhenTypeIsNotDefined()
    {
        // Arrange
        BlobResource? blobResource = BlobResource.Create(nameof(BlobResourceType.Photo)).Value;
        const string newType = "blkdbjldkjbg";

        // Act
        VoidResult result = blobResource.ChangeType(newType);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid blob resource type");
    }
}
