using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Domain.ProductAggregate.Enums;
using FluentAssertions;
using Shared.Domain.Models;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product.Entities;

public class ReviewTests
{
    private ProductAggregate CreateTestProduct()
        => ProductAggregate.Create(
            "Test Product",
            "Test Description",
            100, ProductStatus.InStock,
            Guid.NewGuid(),
            Guid.NewGuid()
            ).Value!;
    
    [Fact]
    public void Create_Should_ReturnSuccessResult_When_AllParametersAreValid()
    {
        // Arrange
        var title = "Great product!";
        var text = "I really enjoyed using this product. It exceeded my expectations.";
        uint rating = 5;
        ProductAggregate product = CreateTestProduct();
        var customerId = Guid.NewGuid();

        // Act
        Result<Review> result = Review.Create(title, text, rating, product, customerId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be(title);
        result.Value.Text.Should().Be(text);
        result.Value.Rating.Should().Be(rating);
        result.Value.Product.Should().Be(product);
        result.Value.CustomerId.Should().Be(customerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ReturnFailureResult_When_TitleIsInvalid(string invalidTitle)
    {
        // Arrange
        var text = "I really enjoyed using this product. It exceeded my expectations.";
        uint rating = 5;
        ProductAggregate product = CreateTestProduct();
        var customerId = Guid.NewGuid();

        // Act
        Result<Review> result = Review.Create(invalidTitle, text, rating, product, customerId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ReturnFailureResult_When_TextIsInvalid(string invalidText)
    {
        // Arrange
        var title = "Great product!";
        uint rating = 5;
        ProductAggregate product = CreateTestProduct();
        var customerId = Guid.NewGuid();

        // Act
        Result<Review> result = Review.Create(title, invalidText, rating, product, customerId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_Should_ReturnFailureResult_When_RatingIsInvalid(uint invalidRating)
    {
        // Arrange
        var title = "Great product!";
        var text = "I really enjoyed using this product. It exceeded my expectations.";
        ProductAggregate product = CreateTestProduct();
        var customerId = Guid.NewGuid();

        // Act
        Result<Review> result = Review.Create(title, text, invalidRating, product, customerId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("rating must be between 1 and 5");
    }

    [Fact]
    public void Create_Should_ReturnFailureResult_When_CustomerIdIsEmpty()
    {
        // Arrange
        var title = "Great product!";
        var text = "I really enjoyed using this product. It exceeded my expectations.";
        uint rating = 5;
        ProductAggregate product = CreateTestProduct();
        var customerId = Guid.Empty;

        // Act
        Result<Review> result = Review.Create(title, text, rating, product, customerId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("customerId is required");
    }

    [Fact]
    public void SetTitle_Should_ReturnSuccessResult_When_TitleIsValid()
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;
        var newTitle = "Updated Title";

        // Act
        VoidResult result = review.SetTitle(newTitle);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Title.Should().Be(newTitle);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetTitle_Should_ReturnFailureResult_When_TitleIsInvalid(string invalidTitle)
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;

        // Act
        VoidResult result = review.SetTitle(invalidTitle);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetText_Should_ReturnSuccessResult_When_TextIsValid()
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;
        var newText = "Updated Text";

        // Act
        VoidResult result = review.SetText(newText);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Text.Should().Be(newText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetText_Should_ReturnFailureResult_When_TextIsInvalid(string invalidText)
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;

        // Act
        VoidResult result = review.SetText(invalidText);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SetRating_Should_ReturnSuccessResult_When_RatingIsValid()
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;
        uint newRating = 3;

        // Act
        VoidResult result = review.SetRating(newRating);

        // Assert
        result.IsSuccess.Should().BeTrue();
        review.Rating.Should().Be(newRating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void SetRating_Should_ReturnFailureResult_When_RatingIsInvalid(uint invalidRating)
    {
        // Arrange
        Review review = Review.Create("Initial Title", "Initial Text", 4, CreateTestProduct(), Guid.NewGuid()).Value;

        // Act
        VoidResult result = review.SetRating(invalidRating);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("rating must be between 1 and 5");
    }

    [Fact]
    public void ExplicitOperator_Should_ConvertReviewToReviewInfo()
    {
        // Arrange
        Review review = Review.Create("Title", "Text", 5, CreateTestProduct(), Guid.NewGuid()).Value;

        // Act
        var reviewInfo = (ReviewInfo)review;

        // Assert
        reviewInfo.Should().NotBeNull();
        reviewInfo.Id.Should().Be(review.Id);
        reviewInfo.Title.Should().Be(review.Title);
        reviewInfo.Text.Should().Be(review.Text);
        reviewInfo.Rating.Should().Be(review.Rating);
        reviewInfo.CustomerId.Should().Be(review.CustomerId);
        reviewInfo.CreatedAt.Should().Be(review.CreatedAt);
    }
}
