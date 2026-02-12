using FluentAssertions;
using Shared.Domain.Models;
using Support.Domain.Entities.SupportRequests;
using Support.Domain.Enums;
using Support.Domain.Projections;

namespace Support.Tests.Domain.SupportRequest;

public class GeneralSupportRequestTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        // Act
        Result<GeneralSupportRequest> result = GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.FullName.FirstName.Should().Be(firstName);
        result.Value.PhoneNumber.Value.Should().Be(phoneNumber);
        result.Value.BirthDate.Should().NotBeNull();
        result.Value.BirthDate!.Value.Should().Be(birthDate);
        result.Value.CustomerId.Should().Be(customerId);
        result.Value.Category.Should().Be(SupportRequestCategory.General);
        result.Value.RequestContent.Should().Be(content);
        result.Value.Status.Should().Be(SupportRequestStatus.Pending);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        string firstName = ""; // Invalid
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        // Act
        Result<GeneralSupportRequest> result = GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "invalid";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        // Act
        Result<GeneralSupportRequest> result = GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenBirthDateIsInFuture()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        // Act
        Result<GeneralSupportRequest> result = GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenCategoryIsInvalid()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = "InvalidCategory";
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        // Act
        Result<GeneralSupportRequest> result = GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Category is required");
    }

    [Fact]
    public void Respond_ShouldSucceed_WhenRequestIsPending()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        Guid supportId = Guid.NewGuid();
        string responseContent = "Here is the solution";

        // Act
        VoidResult result = request.Respond(supportId, responseContent);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.Status.Should().Be(SupportRequestStatus.Processed);
        request.SupportId.Should().Be(supportId);
        request.ResponseContent.Should().Be(responseContent);
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Respond_ShouldFail_WhenRequestIsAlreadyProcessed()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        request.Respond(Guid.NewGuid(), "First response");

        // Act
        VoidResult result = request.Respond(Guid.NewGuid(), "Second response");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Request must be not processed yet to be responded");
    }
    
    [Fact]
    public void Respond_ShouldFail_WhenSupportIdIsEmpty()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        Guid supportId = Guid.Empty;
        string responseContent = "Response";

        // Act
        VoidResult result = request.Respond(supportId, responseContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Support id is required");
    }

    [Fact]
    public void Respond_ShouldFail_WhenResponseContentIsEmpty()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        Guid supportId = Guid.NewGuid();
        string responseContent = "";

        // Act
        VoidResult result = request.Respond(supportId, responseContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Response content is required");
    }

    [Fact]
    public void Cancel_ShouldSucceed_WhenRequestIsPending()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        Guid supportId = Guid.NewGuid();

        // Act
        VoidResult result = request.Cancel(supportId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.Status.Should().Be(SupportRequestStatus.Canceled);
        request.SupportId.Should().Be(supportId);
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_ShouldFail_WhenRequestIsAlreadyProcessed()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        request.Respond(Guid.NewGuid(), "Response");

        // Act
        VoidResult result = request.Cancel(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Request must be not processed yet to be canceled");
    }
    
    [Fact]
    public void Cancel_ShouldFail_WhenSupportIdIsEmpty()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();
        Guid supportId = Guid.Empty;

        // Act
        VoidResult result = request.Cancel(supportId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Support id is required");
    }

    [Fact]
    public void GetAdditionalProperties_ShouldReturnEmptyList()
    {
        // Arrange
        GeneralSupportRequest request = CreateValidRequest();

        // Act
        List<SupportRequestAdditionalProperty> properties = request.GetAdditionalProperties();

        // Assert
        properties.Should().BeEmpty();
    }

    private GeneralSupportRequest CreateValidRequest()
    {
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.General.ToString();
        string content = "I need help";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();

        return GeneralSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements).Value!;
    }
}