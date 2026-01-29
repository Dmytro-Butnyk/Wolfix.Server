using System.Net;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Domain.ProductAggregate.Enums;
using FluentAssertions;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Xunit;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product.Entities;

public class ProductMediaTests
{
    private ProductAggregate CreateProduct()
    {
        Result<ProductAggregate> productResult = ProductAggregate.Create(
            "Test Product",
            "Test Description",
            100,
            ProductStatus.InStock,
            Guid.NewGuid(),
            Guid.NewGuid());

        return productResult.Value!;
    }

    [Fact]
    public void Create_Should_ReturnSuccessResult_With_ProductMedia()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Photo;
        var mediaUrl = "http://example.com/image.png";
        var isMain = true;

        // Act
        Result<ProductMedia> result = ProductMedia.Create(product, mediaId, mediaType, mediaUrl, isMain);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().Be(product.Id);
        result.Value.MediaId.Should().Be(mediaId);
        result.Value.MediaType.Should().Be(mediaType);
        result.Value.MediaUrl.Should().Be(mediaUrl);
        result.Value.IsMain.Should().Be(isMain);
    }

    [Fact]
    public void SetIsMain_Should_Update_IsMain_Property()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Photo;
        var mediaUrl = "http://example.com/image.png";
        var initialIsMain = false;
        Result<ProductMedia> result = ProductMedia.Create(product, mediaId, mediaType, mediaUrl, initialIsMain);
        ProductMedia productMedia = result.Value;
        var newIsMain = true;

        // Act
        productMedia.SetIsMain(newIsMain);

        // Assert
        productMedia.IsMain.Should().Be(newIsMain);
    }

    [Fact]
    public void ExplicitOperator_Should_Return_ProductMediaInfo()
    {
        // Arrange
        ProductAggregate product = CreateProduct();
        var mediaId = Guid.NewGuid();
        var mediaType = BlobResourceType.Photo;
        var mediaUrl = "http://example.com/image.png";
        var isMain = true;
        Result<ProductMedia> result = ProductMedia.Create(product, mediaId, mediaType, mediaUrl, isMain);
        ProductMedia productMedia = result.Value;

        // Act
        var productMediaInfo = (ProductMediaInfo)productMedia;

        // Assert
        productMediaInfo.Should().NotBeNull();
        productMediaInfo.Id.Should().Be(productMedia.Id);
        productMediaInfo.MediaId.Should().Be(mediaId);
        productMediaInfo.MediaType.Should().Be(mediaType);
        productMediaInfo.MediaUrl.Should().Be(mediaUrl);
        productMediaInfo.IsMain.Should().Be(isMain);
    }
}