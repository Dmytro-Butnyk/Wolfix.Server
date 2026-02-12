using FluentAssertions;
using Order.Domain.DeliveryAggregate;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Models;
using Xunit;

namespace Order.Tests.Domain.Delivery.Entities;

public class CityTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenNameIsValid()
    {
        // Arrange
        string name = "New York";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;

        // Act
        Result<City> result = City.Create(name, deliveryMethod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be(name);
        result.Value.DeliveryMethod.Should().Be(deliveryMethod);
        result.Value.DeliveryMethodId.Should().Be(deliveryMethod.Id);
        result.Value.Departments.Should().BeEmpty();
        result.Value.PostMachines.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldReturnFailure_WhenNameIsNullOrEmpty(string? name)
    {
        // Arrange
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;

        // Act
        Result<City> result = City.Create(name!, deliveryMethod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("name cannot be null or empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsTooLong()
    {
        // Arrange
        string name = new string('a', 101);
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;

        // Act
        Result<City> result = City.Create(name, deliveryMethod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("name cannot be longer than 100 characters");
    }

    [Fact]
    public void ExplicitOperator_ShouldReturnCityInfo_WhenCityIsValid()
    {
        // Arrange
        string name = "London";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Standard").Value!;
        City city = City.Create(name, deliveryMethod).Value!;

        // Act
        CityInfo cityInfo = (CityInfo)city;

        // Assert
        cityInfo.Should().NotBeNull();
        cityInfo.Id.Should().Be(city.Id);
        cityInfo.Name.Should().Be(city.Name);
    }
}