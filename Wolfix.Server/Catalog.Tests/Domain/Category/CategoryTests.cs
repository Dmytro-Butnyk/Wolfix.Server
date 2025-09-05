using System.Net;
using FluentAssertions;

namespace Catalog.Tests.Domain.Category;

public sealed class CategoryTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Fail_When_Name_Is_Null_Or_White_Space(string? name)
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create(name);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("text is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Value.Should().BeNull();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Fail_When_Description_Provided_And_Empty_Or_White_Space(string? description)
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", description);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("text is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Value.Should().BeNull();
    }
    
    [Fact]
    public void Create_Should_Success_When_Name_Not_Null_Or_White_Space()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name");
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNull();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Change_Name_Should_Fail_When_Name_Is_Null_Or_White_Space(string? name)
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name").Value!.ChangeName(name);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("text is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public void Change_Name_Should_Success_When_Name_Not_Null_Or_White_Space()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name").Value!.ChangeName("new name");
        
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Change_Description_Should_Fail_When_Description_Is_Empty_Or_White_Space(string? description)
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.ChangeDescription(description);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("description cannot be empty or white space");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Change_Description_Should_Success_When_Description_Not_Empty_Or_White_Space()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.ChangeDescription("new description");
        
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }
    
    [Fact]
    public void Add_Product_Id_Should_Fail_When_Product_Id_Is_Empty()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.AddProductId(Guid.Empty);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("guid is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Add_Product_Id_Should_Fail_When_Id_Already_Exists()
    {
        var productId = Guid.NewGuid();
        var category = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!;
        
        var firstAdding = category.AddProductId(productId);
        
        var secondAdding = category.AddProductId(productId);
        
        secondAdding.IsSuccess.Should().BeFalse();
        secondAdding.ErrorMessage.Should().Be("productId already exists");
        secondAdding.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public void Remove_Product_Id_Should_Fail_When_Product_Id_Is_Empty()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.RemoveProductId(Guid.Empty);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("guid is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Remove_Product_Id_Should_Fail_When_Id_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.RemoveProductId(Guid.NewGuid());
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("productId does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public void GetProductVariant_Should_Fail_When_ProductVariantId_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.GetProductVariant(Guid.NewGuid());
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("productVariant is null. Nothing to get.");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void AddProductVariant_Should_Fail_When_ProductVariant_With_This_Key_Already_Exists()
    {
        var key = "KEY";
        
        var category = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!;
        
        var firstAdding = category.AddProductVariant(key);
        
        var secondAdding = category.AddProductVariant(key);
        
        secondAdding.IsSuccess.Should().BeFalse();
        secondAdding.ErrorMessage.Should().Be("existingProductVariant already exists");
        secondAdding.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public void AddProductVariant_Should_Success_When_ProductVariant_With_This_Key_Not_Exists()
    {
        var key = "KEY";
        
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.AddProductVariant(key);
        
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ChangeProductVariantKey_Should_Fail_When_ProductVariantId_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.ChangeProductVariantKey(Guid.NewGuid(), "new key");
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("existingProductVariant does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void GetProductAttribute_Should_Fail_When_ProductAttributeId_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.GetProductAttribute(Guid.NewGuid());
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("productAttribute is null. Nothing to get.");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void AddProductAttribute_Should_Fail_When_ProductAttribute_With_This_Key_Already_Exists()
    {
        var key = "KEY";
        
        var category = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!;
        
        var firstAdding = category.AddProductAttribute(key);
        
        var secondAdding = category.AddProductAttribute(key);
        
        secondAdding.IsSuccess.Should().BeFalse();
        secondAdding.ErrorMessage.Should().Be("existingProductAttribute already exists");
        secondAdding.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public void AddProductAttribute_Should_Success_When_ProductAttribute_With_This_Key_Not_Exists()
    {
        var key = "KEY";
        
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.AddProductAttribute(key);
        
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }
    
    [Fact]
    public void RemoveProductAttribute_Should_Fail_When_ProductAttributeId_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.RemoveProductAttribute(Guid.NewGuid());
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("existingProductAttribute does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void ChangeProductAttributeKey_Should_Fail_When_ProductAttributeId_Not_Exists()
    {
        var result = Catalog.Domain.CategoryAggregate.Category.Create("name", "description").Value!.ChangeProductAttributeKey(Guid.NewGuid(), "new key");
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("existingProductAttribute does not exist");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    //todo: настроить CI/CD + дописать тесты начиная уже от агрегата продакт (и его сущностей)
    
}