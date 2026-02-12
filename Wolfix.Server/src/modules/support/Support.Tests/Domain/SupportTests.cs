using FluentAssertions;
using Shared.Domain.Models;

namespace Support.Tests.Domain;

public class SupportTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";

        // Act
        Result<Support.Domain.Entities.Support> result = Support.Domain.Entities.Support.Create(accountId, firstName, lastName, middleName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccountId.Should().Be(accountId);
        result.Value.FullName.FirstName.Should().Be(firstName);
        result.Value.FullName.LastName.Should().Be(lastName);
        result.Value.FullName.MiddleName.Should().Be(middleName);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenAccountIdIsEmpty()
    {
        // Arrange
        Guid accountId = Guid.Empty;
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";

        // Act
        Result<Support.Domain.Entities.Support> result = Support.Domain.Entities.Support.Create(accountId, firstName, lastName, middleName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Account id cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFullNameIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        string firstName = ""; // Invalid
        string lastName = "Doe";
        string middleName = "Smith";

        // Act
        Result<Support.Domain.Entities.Support> result = Support.Domain.Entities.Support.Create(accountId, firstName, lastName, middleName);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}