using FluentAssertions;
using Order.Domain.DeliveryAggregate;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Models;
using Xunit;

namespace Order.Tests.Domain.Delivery;

public class DeliveryMethodTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenNameIsNullOrEmpty(string? name)
    {
        // Arrange
        // name is provided by InlineData

        // Act
        Result<DeliveryMethod> result = DeliveryMethod.Create(name!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("name cannot be null or empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsLongerThan100Characters()
    {
        // Arrange
        string name = new string('a', 101);

        // Act
        Result<DeliveryMethod> result = DeliveryMethod.Create(name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("name cannot be longer than 100 characters");
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenNameIsValid()
    {
        // Arrange
        string name = "Valid Name";

        // Act
        Result<DeliveryMethod> result = DeliveryMethod.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be(name);
        result.Value.Cities.Should().BeEmpty();
    }
}