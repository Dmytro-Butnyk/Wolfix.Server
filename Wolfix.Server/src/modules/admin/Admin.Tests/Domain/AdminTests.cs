using System;
using Admin.Domain.AdminAggregate.Enums;
using FluentAssertions;
using Shared.Domain.Models;
using Xunit;

namespace Admin.Tests.Domain;

public class AdminTests
{
    [Fact]
    public void Create_Should_ReturnSuccessResult_When_AllParametersAreValid()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var middleName = "Smith";
        var phoneNumber = "+380471285935";

        // Act
        Result<Admin.Domain.AdminAggregate.Admin> result = Admin.Domain.AdminAggregate.Admin.Create(accountId, firstName, lastName, middleName, phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccountId.Should().Be(accountId);
        result.Value.FullName.FirstName.Should().Be(firstName);
        result.Value.FullName.LastName.Should().Be(lastName);
        result.Value.FullName.MiddleName.Should().Be(middleName);
        result.Value.PhoneNumber.Value.Should().Be(phoneNumber);
        result.Value.Type.Should().Be(AdminType.Basic);
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_When_AccountIdIsEmpty()
    {
        // Arrange
        var accountId = Guid.Empty;
        var firstName = "John";
        var lastName = "Doe";
        var middleName = "Smith";
        var phoneNumber = "+1234567890";

        // Act
        Result<Admin.Domain.AdminAggregate.Admin> result = Admin.Domain.AdminAggregate.Admin.Create(accountId, firstName, lastName, middleName, phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be($"{nameof(accountId)} cannot be empty");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_When_FirstNameIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        var firstName = ""; // Invalid first name
        var lastName = "Doe";
        var middleName = "Smith";
        var phoneNumber = "+1234567890";

        // Act
        Result<Admin.Domain.AdminAggregate.Admin> result = Admin.Domain.AdminAggregate.Admin.Create(accountId, firstName, lastName, middleName, phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        // TODO: Add specific error message check for invalid first name if FullName.Create provides it
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_When_PhoneNumberIsInvalid()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var middleName = "Smith";
        var phoneNumber = "invalid-phone"; // Invalid phone number

        // Act
        Result<Admin.Domain.AdminAggregate.Admin> result = Admin.Domain.AdminAggregate.Admin.Create(accountId, firstName, lastName, middleName, phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        // TODO: Add specific error message check for invalid phone number if PhoneNumber.Create provides it
    }
}