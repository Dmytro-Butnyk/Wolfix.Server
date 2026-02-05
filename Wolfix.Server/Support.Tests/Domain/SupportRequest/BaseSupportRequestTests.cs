using FluentAssertions;
using Shared.Domain.Models;
using Support.Domain.Entities.SupportRequests;
using Support.Domain.Enums;

namespace Support.Tests.Domain.SupportRequest;

public class BaseSupportRequestTests
{
    [Fact]
    public void ValidateCreateData_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly? birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";

        // Act
        Result<SupportRequestCreateData> result = BaseSupportRequest.ValidateCreateData(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FullName.FirstName.Should().Be(firstName);
        result.Value.PhoneNumber.Value.Should().Be(phoneNumber);
        result.Value.BirthDate.Should().NotBeNull();
        result.Value.BirthDate!.Value.Should().Be(birthDate);
        result.Value.Category.Should().Be(SupportRequestCategory.General);
    }

    [Fact]
    public void ValidateCreateData_ShouldReturnFailure_WhenCustomerIdIsEmpty()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly? birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.Empty;
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";

        // Act
        Result<SupportRequestCreateData> result = BaseSupportRequest.ValidateCreateData(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Customer Id is required");
    }

    [Fact]
    public void ValidateCreateData_ShouldReturnFailure_WhenContentIsEmpty()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly? birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "";

        // Act
        Result<SupportRequestCreateData> result = BaseSupportRequest.ValidateCreateData(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("RequestContent is required");
    }

    [Fact]
    public void ValidateCreateData_ShouldReturnFailure_WhenCategoryIsInvalid()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly? birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = "InvalidCategory";
        string content = "I need help";

        // Act
        Result<SupportRequestCreateData> result = BaseSupportRequest.ValidateCreateData(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Category is required");
    }
}