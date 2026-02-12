using FluentAssertions;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.Models;
using Xunit;
using SellerAggregate = Seller.Domain.SellerAggregate;

namespace Seller.Tests.Domain.Seller.Entities;

public class SellerCategoryTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenInputsAreValid()
    {
        // Arrange
        SellerAggregate.Seller seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string name = "Electronics";

        // Act
        Result<SellerCategory> result = SellerCategory.Create(seller, categoryId, name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryId.Should().Be(categoryId);
        result.Value.Name.Should().Be(name);
        result.Value.Seller.Should().Be(seller);
        result.Value.SellerId.Should().Be(seller.Id);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Arrange
        SellerAggregate.Seller seller = CreateSeller();
        Guid categoryId = Guid.Empty;
        string name = "Electronics";

        // Act
        Result<SellerCategory> result = SellerCategory.Create(seller, categoryId, name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("categoryId");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        SellerAggregate.Seller seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string name = "";

        // Act
        Result<SellerCategory> result = SellerCategory.Create(seller, categoryId, name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("name");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsWhitespace()
    {
        // Arrange
        SellerAggregate.Seller seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string name = "   ";

        // Act
        Result<SellerCategory> result = SellerCategory.Create(seller, categoryId, name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("name");
    }

    [Fact]
    public void ExplicitOperator_ShouldReturnSellerCategoryInfo_WhenSellerCategoryIsValid()
    {
        // Arrange
        SellerAggregate.Seller seller = CreateSeller();
        Guid categoryId = Guid.NewGuid();
        string name = "Electronics";
        SellerCategory sellerCategory = SellerCategory.Create(seller, categoryId, name).Value!;

        // Act
        SellerCategoryInfo info = (SellerCategoryInfo)sellerCategory;

        // Assert
        info.Should().NotBeNull();
        info.Id.Should().Be(sellerCategory.Id);
        info.CategoryId.Should().Be(sellerCategory.CategoryId);
        info.Name.Should().Be(sellerCategory.Name);
        info.SellerId.Should().Be(sellerCategory.SellerId);
    }

    private static SellerAggregate.Seller CreateSeller()
    {
        return SellerAggregate.Seller.Create(
            Guid.NewGuid(), 
            "John", "Doe", "Smith", 
            "+380123456789", 
            "Kyiv", "Street", 1, null, 
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))).Value!;
    }
}