using FluentAssertions;
using Shared.Domain.Models;
using Support.Domain.Entities.SupportRequests;
using Support.Domain.Enums;
using Support.Domain.Projections;

namespace Support.Tests.Domain.SupportRequest;

public class OrderIssueSupportRequestTests
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
        string category = SupportRequestCategory.OrderIssue.ToString();
        string content = "Issue with order";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        Guid orderId = Guid.NewGuid();
        string orderNumber = "ORD-123";

        // Act
        Result<OrderIssueSupportRequest> result = OrderIssueSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, orderId, orderNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.OrderId.Should().Be(orderId);
        result.Value.OrderNumber.Should().Be(orderNumber);
        result.Value.Category.Should().Be(SupportRequestCategory.OrderIssue);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenOrderIdIsEmpty()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.OrderIssue.ToString();
        string content = "Issue with order";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        Guid orderId = Guid.Empty;
        string orderNumber = "ORD-123";

        // Act
        Result<OrderIssueSupportRequest> result = OrderIssueSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, orderId, orderNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("order id cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenOrderNumberIsEmpty()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.OrderIssue.ToString();
        string content = "Issue with order";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        Guid orderId = Guid.NewGuid();
        string orderNumber = "";

        // Act
        Result<OrderIssueSupportRequest> result = OrderIssueSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, orderId, orderNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("order number cannot be null or empty");
    }

    [Fact]
    public void GetAdditionalProperties_ShouldReturnOrderDetails()
    {
        // Arrange
        OrderIssueSupportRequest request = CreateValidRequest();

        // Act
        List<SupportRequestAdditionalProperty> properties = request.GetAdditionalProperties();

        // Assert
        properties.Should().HaveCount(2);
        properties.Should().Contain(p => p.Name == "OrderId" && p.Value == request.OrderId.ToString());
        properties.Should().Contain(p => p.Name == "OrderNumber" && p.Value == request.OrderNumber);
    }

    private OrderIssueSupportRequest CreateValidRequest()
    {
        string firstName = "John";
        string lastName = "Doe";
        string middleName = "Smith";
        string phoneNumber = "+380123456789";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        Guid customerId = Guid.NewGuid();
        string category = SupportRequestCategory.OrderIssue.ToString();
        string content = "Issue with order";
        Dictionary<string, object> extraElements = new Dictionary<string, object>();
        Guid orderId = Guid.NewGuid();
        string orderNumber = "ORD-123";

        return OrderIssueSupportRequest.Create(firstName, lastName, middleName, phoneNumber, birthDate, customerId, category, content, extraElements, orderId, orderNumber).Value!;
    }
}