using FluentAssertions;
using Seller.Domain.SellerApplicationAggregate;
using Seller.Domain.SellerApplicationAggregate.Enums;
using Seller.Domain.SellerApplicationAggregate.ValueObjects;
using Shared.Domain.Models;
using Xunit;
using SellerApplicationAggregate = Seller.Domain.SellerApplicationAggregate.SellerApplication;

namespace Seller.Tests.Domain.SellerApplication;

public class SellerApplicationTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenAccountIdIsEmpty()
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(accountId: Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("accountId cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(categoryId: Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("categoryId cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenCategoryNameIsInvalid(string categoryName)
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(categoryName: categoryName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("categoryName cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenBlobResourceIdIsEmpty()
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(blobResourceId: Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("blobResourceId cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenDocumentUrlIsInvalid(string documentUrl)
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(documentUrl: documentUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("documentUrl cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenSellerProfileDataIsInvalid()
    {
        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(firstName: "");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenAllInputsAreValid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Guid categoryId = Guid.NewGuid();
        string categoryName = "Electronics";
        Guid blobResourceId = Guid.NewGuid();
        string documentUrl = "http://example.com/doc";

        // Act
        Result<SellerApplicationAggregate> result = CreateSellerApplication(
            accountId: accountId,
            categoryId: categoryId,
            categoryName: categoryName,
            blobResourceId: blobResourceId,
            documentUrl: documentUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccountId.Should().Be(accountId);
        result.Value.CategoryId.Should().Be(categoryId);
        result.Value.CategoryName.Should().Be(categoryName);
        result.Value.BlobResourceId.Should().Be(blobResourceId);
        result.Value.DocumentUrl.Should().Be(documentUrl);
        result.Value.Status.Should().Be(SellerApplicationStatus.Pending);
        result.Value.SellerProfileData.Should().NotBeNull();
    }

    [Fact]
    public void Approve_ShouldReturnSuccess_WhenStatusIsPending()
    {
        // Arrange
        SellerApplicationAggregate sellerApplication = CreateSellerApplication().Value!;

        // Act
        VoidResult result = sellerApplication.Approve();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sellerApplication.Status.Should().Be(SellerApplicationStatus.Approved);
    }

    [Fact]
    public void Approve_ShouldReturnFailure_WhenStatusIsNotPending()
    {
        // Arrange
        SellerApplicationAggregate sellerApplication = CreateSellerApplication().Value!;
        sellerApplication.Approve(); // First approval

        // Act
        VoidResult result = sellerApplication.Approve(); // Second approval

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Application is already approved or rejected");
    }

    [Fact]
    public void Reject_ShouldReturnSuccess_WhenStatusIsPending()
    {
        // Arrange
        SellerApplicationAggregate sellerApplication = CreateSellerApplication().Value!;

        // Act
        VoidResult result = sellerApplication.Reject();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sellerApplication.Status.Should().Be(SellerApplicationStatus.Rejected);
    }

    [Fact]
    public void Reject_ShouldReturnFailure_WhenStatusIsNotPending()
    {
        // Arrange
        SellerApplicationAggregate sellerApplication = CreateSellerApplication().Value!;
        sellerApplication.Reject(); // First rejection

        // Act
        VoidResult result = sellerApplication.Reject(); // Second rejection

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Application is already approved or rejected");
    }

    private static Result<SellerApplicationAggregate> CreateSellerApplication(
        Guid? accountId = null,
        Guid? categoryId = null,
        string? categoryName = "Electronics",
        Guid? blobResourceId = null,
        string? documentUrl = "http://example.com/doc",
        string? firstName = "John",
        string? lastName = "Doe",
        string? middleName = "Smith",
        string? phoneNumber = "+380970878346",
        string? city = "New York",
        string? street = "5th Avenue",
        uint houseNumber = 10,
        uint? apartmentNumber = 5,
        DateOnly? birthDate = null)
    {
        return SellerApplicationAggregate.Create(
            accountId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            categoryName!,
            blobResourceId ?? Guid.NewGuid(),
            documentUrl!,
            firstName!,
            lastName!,
            middleName!,
            phoneNumber!,
            city!,
            street!,
            houseNumber,
            apartmentNumber,
            birthDate ?? new DateOnly(1990, 1, 1));
    }
}