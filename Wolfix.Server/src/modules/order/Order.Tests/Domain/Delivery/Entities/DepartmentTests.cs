using FluentAssertions;
using Order.Domain.DeliveryAggregate;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Models;
using Xunit;

namespace Order.Tests.Domain.Delivery.Entities;

public class DepartmentTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        uint number = 1;
        string street = "Main St";
        uint houseNumber = 10;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<Department> result = Department.Create(number, street, houseNumber, city);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Number.Should().Be(number);
        result.Value.Street.Should().Be(street);
        result.Value.HouseNumber.Should().Be(houseNumber);
        result.Value.City.Should().Be(city);
        result.Value.CityId.Should().Be(city.Id);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNumberIsZero()
    {
        // Arrange
        uint number = 0;
        string street = "Main St";
        uint houseNumber = 10;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<Department> result = Department.Create(number, street, houseNumber, city);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("number must be positive");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldReturnFailure_WhenStreetIsNullOrEmpty(string? street)
    {
        // Arrange
        uint number = 1;
        uint houseNumber = 10;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<Department> result = Department.Create(number, street!, houseNumber, city);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("street cannot be null or empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenHouseNumberIsZero()
    {
        // Arrange
        uint number = 1;
        string street = "Main St";
        uint houseNumber = 0;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<Department> result = Department.Create(number, street, houseNumber, city);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("houseNumber must be positive");
    }

    [Fact]
    public void ExplicitOperator_ShouldReturnDepartmentInfo_WhenDepartmentIsValid()
    {
        // Arrange
        uint number = 5;
        string street = "Broadway";
        uint houseNumber = 20;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Standard").Value!;
        City city = City.Create("London", deliveryMethod).Value!;
        Department department = Department.Create(number, street, houseNumber, city).Value!;

        // Act
        DepartmentInfo departmentInfo = (DepartmentInfo)department;

        // Assert
        departmentInfo.Should().NotBeNull();
        departmentInfo.Id.Should().Be(department.Id);
        departmentInfo.Number.Should().Be(department.Number);
        departmentInfo.Street.Should().Be(department.Street);
        departmentInfo.HouseNumber.Should().Be(department.HouseNumber);
    }
}