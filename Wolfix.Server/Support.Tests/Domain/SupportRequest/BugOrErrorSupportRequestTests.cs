using FluentAssertions;
using Shared.Domain.Models;
using Support.Domain.Entities.SupportRequests;
using Support.Domain.Enums;
using Support.Domain.Projections;

namespace Support.Tests.Domain.SupportRequest;

public class BugOrErrorSupportRequestTests
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
        string category = SupportRequestCategory.BugOrError.ToString();
        string content = "I found a bug";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        string photoUrl = "http://example.com/photo.jpg";

        // Act
        Result<BugOrErrorSupportRequest> result = BugOrErrorSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, photoUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.PhotoUrl.Should().Be(photoUrl);
        result.Value.Category.Should().Be(SupportRequestCategory.BugOrError);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPhotoUrlIsEmpty()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.BugOrError.ToString();
        string content = "I found a bug";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        string photoUrl = "";

        // Act
        Result<BugOrErrorSupportRequest> result = BugOrErrorSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, photoUrl);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("photo url cannot be null or white space");
    }

    [Fact]
    public void GetAdditionalProperties_ShouldReturnPhotoUrl()
    {
        // Arrange
        BugOrErrorSupportRequest request = CreateValidRequest();

        // Act
        List<SupportRequestAdditionalProperty> properties = request.GetAdditionalProperties();

        // Assert
        properties.Should().HaveCount(1);
        properties[0].Name.Should().Be("PhotoUrl");
        properties[0].Value.Should().Be(request.PhotoUrl);
    }

    private BugOrErrorSupportRequest CreateValidRequest()
    {
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.BugOrError.ToString();
        string content = "I found a bug";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        string photoUrl = "http://example.com/photo.jpg";

        return BugOrErrorSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, photoUrl).Value!;
    }
}