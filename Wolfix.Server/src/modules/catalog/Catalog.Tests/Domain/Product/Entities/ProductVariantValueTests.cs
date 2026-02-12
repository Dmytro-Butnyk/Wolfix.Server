using System;
using System.Net;
using Catalog.Domain.ProductAggregate.Entities;
using FluentAssertions;
using Xunit;
using Shared.Domain.Models;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using ProductAggregate = Catalog.Domain.ProductAggregate.Product;

namespace Catalog.Tests.Domain.Product.Entities
{
    public class ProductVariantValueTests
    {
        private ProductAggregate CreateProduct()
            => ProductAggregate.Create(
                "title", 
                "description",
                100,
                ProductStatus.InStock,
                Guid.NewGuid(),
                Guid.NewGuid()
            ).Value!;
        
        [Fact]
        public void Create_Should_ReturnFailure_When_KeyIsInvalid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var invalidKey = "";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();

            // Act
            Result<ProductVariantValue> result = ProductVariantValue.Create(product, invalidKey, value, categoryVariantId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Create_Should_ReturnFailure_When_CategoryVariantIdIsEmpty()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var emptyCategoryVariantId = Guid.Empty;

            // Act
            Result<ProductVariantValue> result = ProductVariantValue.Create(product, key, value, emptyCategoryVariantId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("categoryVariantId is required");
        }
        
        [Fact]
        public void Create_Should_ReturnSuccess_When_AllParametersAreValid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();

            // Act
            Result<ProductVariantValue> result = ProductVariantValue.Create(product, key, value, categoryVariantId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public void SetKey_Should_ReturnFailure_When_KeyIsInvalid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();
            Result<ProductVariantValue> initialResult = ProductVariantValue.Create(product, key, value, categoryVariantId);
            ProductVariantValue productVariantValue = initialResult.Value;
            var invalidKey = "";

            // Act
            VoidResult result = productVariantValue.SetKey(invalidKey);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SetKey_Should_ReturnSuccess_And_UpdateKey_When_KeyIsValid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();
            Result<ProductVariantValue> initialResult = ProductVariantValue.Create(product, key, value, categoryVariantId);
            ProductVariantValue productVariantValue = initialResult.Value;
            var newKey = "newKey";

            // Act
            VoidResult result = productVariantValue.SetKey(newKey);
            var info = (ProductVariantValueInfo)productVariantValue;

            // Assert
            result.IsSuccess.Should().BeTrue();
            info.Key.Should().Be(newKey);
        }
        
        [Fact]
        public void SetValue_Should_ReturnFailure_When_ValueIsInvalid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();
            Result<ProductVariantValue> initialResult = ProductVariantValue.Create(product, key, value, categoryVariantId);
            ProductVariantValue productVariantValue = initialResult.Value;
            var invalidValue = "";

            // Act
            VoidResult result = productVariantValue.SetValue(invalidValue);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SetValue_Should_ReturnSuccess_And_UpdateValue_When_ValueIsValid()
        {
            // Arrange
            ProductAggregate product = CreateProduct();
            var key = "key";
            var value = "value";
            var categoryVariantId = Guid.NewGuid();
            Result<ProductVariantValue> initialResult = ProductVariantValue.Create(product, key, value, categoryVariantId);
            ProductVariantValue productVariantValue = initialResult.Value;
            var newValue = "newValue";

            // Act
            VoidResult result = productVariantValue.SetValue(newValue);
            var info = (ProductVariantValueInfo)productVariantValue;

            // Assert
            result.IsSuccess.Should().BeTrue();
            info.Value.Should().Be(newValue);
        }
    }
}