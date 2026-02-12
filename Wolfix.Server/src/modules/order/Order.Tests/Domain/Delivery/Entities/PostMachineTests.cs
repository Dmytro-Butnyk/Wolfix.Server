using FluentAssertions;
using Order.Domain.DeliveryAggregate;
using Order.Domain.DeliveryAggregate.Entities;
using Shared.Domain.Models;
using Xunit;

namespace Order.Tests.Domain.Delivery.Entities;

public class PostMachineTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        uint number = 100;
        string street = "Fifth Ave";
        uint houseNumber = 50;
        string? note = "Near entrance";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<PostMachine> result = PostMachine.Create(number, street, houseNumber, note, city);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Number.Should().Be(number);
        result.Value.Street.Should().Be(street);
        result.Value.HouseNumber.Should().Be(houseNumber);
        result.Value.Note.Should().Be(note);
        result.Value.City.Should().Be(city);
        result.Value.CityId.Should().Be(city.Id);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenNoteIsNull()
    {
        // Arrange
        uint number = 100;
        string street = "Fifth Ave";
        uint houseNumber = 50;
        string? note = null;
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<PostMachine> result = PostMachine.Create(number, street, houseNumber, note, city);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Note.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNumberIsZero()
    {
        // Arrange
        uint number = 0;
        string street = "Fifth Ave";
        uint houseNumber = 50;
        string? note = "Near entrance";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<PostMachine> result = PostMachine.Create(number, street, houseNumber, note, city);

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
        uint number = 100;
        uint houseNumber = 50;
        string? note = "Near entrance";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<PostMachine> result = PostMachine.Create(number, street!, houseNumber, note, city);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("street cannot be null or empty");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenHouseNumberIsZero()
    {
        // Arrange
        uint number = 100;
        string street = "Fifth Ave";
        uint houseNumber = 0;
        string? note = "Near entrance";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Express").Value!;
        City city = City.Create("New York", deliveryMethod).Value!;

        // Act
        Result<PostMachine> result = PostMachine.Create(number, street, houseNumber, note, city);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("houseNumber must be positive");
    }

    [Fact]
    public void ExplicitOperator_ShouldReturnPostMachineInfo_WhenPostMachineIsValid()
    {
        // Arrange
        uint number = 200;
        string street = "Oxford St";
        uint houseNumber = 10;
        string? note = "Backyard";
        DeliveryMethod deliveryMethod = DeliveryMethod.Create("Standard").Value!;
        City city = City.Create("London", deliveryMethod).Value!;
        PostMachine postMachine = PostMachine.Create(number, street, houseNumber, note, city).Value!;

        // Act
        PostMachineInfo postMachineInfo = (PostMachineInfo)postMachine;

        // Assert
        postMachineInfo.Should().NotBeNull();
        postMachineInfo.Id.Should().Be(postMachine.Id);
        postMachineInfo.Number.Should().Be(postMachine.Number);
        postMachineInfo.Street.Should().Be(postMachine.Street);
        postMachineInfo.HouseNumber.Should().Be(postMachine.HouseNumber);
        postMachineInfo.Note.Should().Be(postMachine.Note);
    }
}