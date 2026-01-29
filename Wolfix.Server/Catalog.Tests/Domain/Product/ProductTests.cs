using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using FluentAssertions;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product;

public class ProductTests
{
    private static Result<ProductAggregate> CreateProduct()
    {
        return ProductAggregate.Create("Test Product", "Test Description", 100m, ProductStatus.InStock, Guid.NewGuid(), Guid.NewGuid());
    }
    
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        //arrange
        var title = "Test Product";
        var description = "Test Description";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be(title);
        result.Value.Description.Should().Be(description);
        result.Value.Price.Should().Be(price);
        result.Value.Status.Should().Be(status);
        result.Value.CategoryId.Should().Be(categoryId);
        result.Value.SellerId.Should().Be(sellerId);
        result.Value.FinalPrice.Should().Be(price);
        result.Value.Bonuses.Should().Be(1); // 100 * 0.01
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenTitleIsInvalid(string title)
    {
        //arrange
        var description = "Test Description";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenDescriptionIsInvalid(string description)
    {
        //arrange
        var title = "Test Product";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldReturnFailure_WhenPriceIsInvalid(decimal price)
    {
        //arrange
        var title = "Test Product";
        var description = "Test Description";
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void Create_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        //arrange
        var title = "Test Product";
        var description = "Test Description";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.Empty;
        var sellerId = Guid.NewGuid();

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void Create_ShouldReturnFailure_WhenSellerIdIsEmpty()
    {
        //arrange
        var title = "Test Product";
        var description = "Test Description";
        var price = 100m;
        var status = ProductStatus.InStock;
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.Empty;

        //act
        Result<ProductAggregate> result = ProductAggregate.Create(title, description, price, status, categoryId, sellerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void ChangeTitle_ShouldChangeTitle_WhenTitleIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var newTitle = "New Title";

        //act
        var result = product.ChangeTitle(newTitle);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Title.Should().Be(newTitle);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeTitle_ShouldReturnFailure_WhenTitleIsInvalid(string newTitle)
    {
        //arrange
        var product = CreateProduct().Value;

        //act
        var result = product.ChangeTitle(newTitle);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangeDescription_ShouldChangeDescription_WhenDescriptionIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var newDescription = "New Description";

        //act
        var result = product.ChangeDescription(newDescription);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeDescription_ShouldReturnFailure_WhenDescriptionIsInvalid(string newDescription)
    {
        //arrange
        var product = CreateProduct().Value;

        //act
        var result = product.ChangeDescription(newDescription);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangePrice_ShouldChangePriceAndRecalculateBonuses_WhenPriceIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var newPrice = 200m;

        //act
        var result = product.ChangePrice(newPrice);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(newPrice);
        product.Bonuses.Should().Be(2); // 200 * 0.01
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChangePrice_ShouldReturnFailure_WhenPriceIsInvalid(decimal newPrice)
    {
        //arrange
        var product = CreateProduct().Value;

        //act
        var result = product.ChangePrice(newPrice);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangeStatus_ShouldChangeStatus_WhenStatusIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var newStatus = ProductStatus.NotAvailable;

        //act
        var result = product.ChangeStatus(newStatus);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(newStatus);
    }
    
    [Fact]
    public void ChangeCategory_ShouldChangeCategory_WhenCategoryIdIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var newCategoryId = Guid.NewGuid();

        //act
        var result = product.ChangeCategory(newCategoryId);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void ChangeCategory_ShouldReturnFailure_WhenCategoryIdIsSame()
    {
        //arrange
        var product = CreateProduct().Value;
        var newCategoryId = product.CategoryId;

        //act
        var result = product.ChangeCategory(newCategoryId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangeCategory_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        //arrange
        var product = CreateProduct().Value;
        var newCategoryId = Guid.Empty;

        //act
        var result = product.ChangeCategory(newCategoryId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void AddDiscount_ShouldAddDiscountAndRecalculateFinalPrice_WhenDataIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var percent = 10u;
        var expiration = DateTime.UtcNow.AddDays(1);

        //act
        var result = product.AddDiscount(percent, expiration);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.GetDiscount().IsSuccess.Should().BeTrue();
        product.FinalPrice.Should().Be(90); // 100 * (100 - 10) / 100
    }

    [Fact]
    public void AddDiscount_ShouldReturnFailure_WhenPercentIsInvalid()
    {
        //arrange
        var product = CreateProduct().Value;
        var percent = 101u;
        var expiration = DateTime.UtcNow.AddDays(1);

        //act
        var result = product.AddDiscount(percent, expiration);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void AddDiscount_ShouldReturnFailure_WhenExpirationIsInvalid()
    {
        //arrange
        var product = CreateProduct().Value;
        var percent = 10u;
        var expiration = DateTime.UtcNow.AddDays(-1);

        //act
        var result = product.AddDiscount(percent, expiration);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RemoveDiscount_ShouldRemoveDiscountAndRecalculateFinalPrice()
    {
        //arrange
        var product = CreateProduct().Value;
        product.AddDiscount(10, DateTime.UtcNow.AddDays(1));

        //act
        var result = product.RemoveDiscount();

        //assert
        result.IsSuccess.Should().BeTrue();
        product.GetDiscount().IsSuccess.Should().BeFalse();
        product.FinalPrice.Should().Be(100);
    }

    [Fact]
    public void RemoveDiscount_ShouldReturnFailure_WhenDiscountDoesNotExist()
    {
        //arrange
        var product = CreateProduct().Value;

        //act
        var result = product.RemoveDiscount();

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void MakeDiscountExpired_ShouldMakeDiscountExpiredAndRecalculateFinalPrice()
    {
        //arrange
        var product = CreateProduct().Value;
        product.AddDiscount(10, DateTime.UtcNow.AddDays(1));

        //act
        var result = product.MakeDiscountExpired();

        //assert
        result.IsSuccess.Should().BeTrue();
        product.FinalPrice.Should().Be(100);
    }

    [Fact]
    public void MakeDiscountExpired_ShouldReturnFailure_WhenDiscountDoesNotExist()
    {
        //arrange
        var product = CreateProduct().Value;

        //act
        var result = product.MakeDiscountExpired();

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void AddProductMedia_ShouldAddMedia_WhenDataIsValid()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Photo;
        var mediaUrl = "http://example.com/image.jpg";

        //act
        var result = product.AddProductMedia(mediaId, mediaType, mediaUrl, false);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductMedias.Should().HaveCount(1);
    }

    [Fact]
    public void AddProductMedia_ShouldSetFirstImageAsMain_WhenNoMainPhotoExists()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Photo;
        var mediaUrl = "http://example.com/image.jpg";

        //act
        product.AddProductMedia(mediaId, mediaType, mediaUrl, false);

        //assert
        product.MainPhotoUrl.Should().Be(mediaUrl);
    }

    [Fact]
    public void AddProductMedia_ShouldReturnFailure_WhenFirstMediaIsVideo()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Video;
        var mediaUrl = "http://example.com/video.mp4";

        //act
        var result = product.AddProductMedia(mediaId, mediaType, mediaUrl, false);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RemoveProductMedia_ShouldRemoveMedia()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId1 = Guid.NewGuid();
        var mediaId2 = Guid.NewGuid();
        product.AddProductMedia(mediaId1, BlobResourceType.Photo, "url1", true);
        product.AddProductMedia(mediaId2, BlobResourceType.Photo, "url2", false);

        //act
        var result = product.RemoveProductMedia(product.ProductMedias.First(x => x.MediaId == mediaId2).Id);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductMedias.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveProductMedia_ShouldReturnFailure_WhenMediaIsMain()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId = Guid.NewGuid();
        product.AddProductMedia(mediaId, BlobResourceType.Photo, "url", true);

        //act
        var result = product.RemoveProductMedia(product.ProductMedias.First().Id);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangeMainPhoto_ShouldChangeMainPhoto()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId1 = Guid.NewGuid();
        var mediaId2 = Guid.NewGuid();
        product.AddProductMedia(mediaId1, BlobResourceType.Photo, "url1", true);
        product.AddProductMedia(mediaId2, BlobResourceType.Photo, "url2", false);
        var newMainPhotoId = product.ProductMedias.First(x => x.MediaId == mediaId2).Id;

        //act
        var result = product.ChangeMainPhoto(newMainPhotoId);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.MainPhotoUrl.Should().Be("url2");
    }

    [Fact]
    public void ChangeMainPhoto_ShouldReturnFailure_WhenNewMainPhotoIsVideo()
    {
        //arrange
        var product = CreateProduct().Value;
        var mediaId1 = Guid.NewGuid();
        var mediaId2 = Guid.NewGuid();
        product.AddProductMedia(mediaId1, BlobResourceType.Photo, "url1", true);
        product.AddProductMedia(mediaId2, BlobResourceType.Video, "url2", false);
        var newMainPhotoId = product.ProductMedias.First(x => x.MediaId == mediaId2).Id;

        //act
        var result = product.ChangeMainPhoto(newMainPhotoId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void AddReview_ShouldAddReviewAndRecalculateAverageRating()
    {
        //arrange
        var product = CreateProduct().Value;
        var title = "Great product!";
        var text = "I really liked it.";
        var rating = 5u;
        var customerId = Guid.NewGuid();

        //act
        var result = product.AddReview(title, text, rating, customerId);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Reviews.Should().HaveCount(1);
        product.AverageRating.Should().Be(5);
    }

    [Fact]
    public void AddReview_ShouldReturnFailure_WhenRatingIsInvalid()
    {
        //arrange
        var product = CreateProduct().Value;
        var title = "Bad product!";
        var text = "I didn't like it.";
        var rating = 6u;
        var customerId = Guid.NewGuid();

        //act
        var result = product.AddReview(title, text, rating, customerId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RemoveReview_ShouldRemoveReviewAndRecalculateAverageRating()
    {
        //arrange
        var product = CreateProduct().Value;
        product.AddReview("Title 1", "Text 1", 5, Guid.NewGuid());
        product.AddReview("Title 2", "Text 2", 3, Guid.NewGuid());
        var reviewIdToRemove = product.Reviews.First().Id;

        //act
        var result = product.RemoveReview(reviewIdToRemove);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.Reviews.Should().HaveCount(1);
        product.AverageRating.Should().Be(3);
    }

    [Fact]
    public void ChangeReviewRating_ShouldChangeRatingAndRecalculateAverageRating()
    {
        //arrange
        var product = CreateProduct().Value;
        product.AddReview("Title 1", "Text 1", 5, Guid.NewGuid());
        var reviewId = product.Reviews.First().Id;
        var newRating = 1u;

        //act
        var result = product.ChangeReviewRating(reviewId, newRating);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.AverageRating.Should().Be(1);
    }
    
    [Fact]
    public void AddProductAttributeValue_ShouldAddAttribute()
    {
        //arrange
        var product = CreateProduct().Value;
        var key = "Color";
        var value = "Red";
        var attributeId = Guid.NewGuid();

        //act
        var result = product.AddProductAttributeValue(key, value, attributeId);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductAttributeValues.Should().HaveCount(1);
    }

    [Fact]
    public void AddProductAttributeValue_ShouldReturnFailure_WhenAttributeAlreadyExists()
    {
        //arrange
        var product = CreateProduct().Value;
        var key = "Color";
        var value = "Red";
        var attributeId = Guid.NewGuid();
        product.AddProductAttributeValue(key, value, attributeId);

        //act
        var result = product.AddProductAttributeValue(key, value, attributeId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RemoveProductAttributeValue_ShouldRemoveAttribute()
    {
        //arrange
        var product = CreateProduct().Value;
        var attributeId = Guid.NewGuid();
        product.AddProductAttributeValue("Color", "Red", attributeId);

        //act
        product.RemoveProductAttributeValue(attributeId);

        //assert
        product.ProductAttributeValues.Should().BeEmpty();
    }

    [Fact]
    public void ChangeProductAttributeValue_ShouldChangeValue()
    {
        //arrange
        var product = CreateProduct().Value;
        var attributeId = Guid.NewGuid();
        product.AddProductAttributeValue("Color", "Red", attributeId);
        var attributeValueId = product.ProductAttributeValues.First().Id;
        var newValue = "Blue";

        //act
        var result = product.ChangeProductAttributeValue(attributeValueId, newValue);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductAttributeValues.First().Value.Should().Be(newValue);
    }
    
    [Fact]
    public void AddProductVariantValue_ShouldAddVariant()
    {
        //arrange
        var product = CreateProduct().Value;
        var key = "Size";
        var value = "XL";
        var variantId = Guid.NewGuid();

        //act
        var result = product.AddProductVariantValue(key, value, variantId);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductVariantValues.Should().HaveCount(1);
    }

    [Fact]
    public void AddProductVariantValue_ShouldReturnFailure_WhenVariantAlreadyExists()
    {
        //arrange
        var product = CreateProduct().Value;
        var key = "Size";
        var value = "XL";
        var variantId = Guid.NewGuid();
        product.AddProductVariantValue(key, value, variantId);

        //act
        var result = product.AddProductVariantValue(key, value, variantId);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RemoveProductVariantValue_ShouldRemoveVariant()
    {
        //arrange
        var product = CreateProduct().Value;
        var variantId = Guid.NewGuid();
        product.AddProductVariantValue("Size", "XL", variantId);

        //act
        product.RemoveProductVariantValue(variantId);

        //assert
        product.ProductVariantValues.Should().BeEmpty();
    }

    [Fact]
    public void ChangeProductVariantValue_ShouldChangeValue()
    {
        //arrange
        var product = CreateProduct().Value;
        var variantId = Guid.NewGuid();
        product.AddProductVariantValue("Size", "XL", variantId);
        var variantValueId = product.ProductVariantValues.First().Id;
        var newValue = "XXL";

        //act
        var result = product.ChangeProductVariantValue(variantValueId, newValue);

        //assert
        result.IsSuccess.Should().BeTrue();
        product.ProductVariantValues.First().Value.Should().Be(newValue);
    }
}
