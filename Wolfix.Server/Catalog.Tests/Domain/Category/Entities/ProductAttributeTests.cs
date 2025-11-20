using System.Net;
using Catalog.Domain.CategoryAggregate.Entities;
using FluentAssertions;

namespace Catalog.Tests.Domain.Category.Entities;

public sealed class ProductAttributeTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Fail_If_Key_Is_Null_Or_White_Space(string? key)
    {
        var result = ProductAttribute.Create(null, key);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("key is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Value.Should().BeNull();
    }
    
    [Fact]
    public void Create_Should_Success_If_Key_Is_Not_Null_Or_White_Space()
    {
        var result = ProductAttribute.Create(null, "key");
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNull();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Set_Key_Should_Fail_If_Key_Is_Null_Or_White_Space(string? key)
    {
        var result = ProductAttribute.Create(null, "key").Value.SetKey(key);
        
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("key is required");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Set_Key_Should_Success_If_Key_Is_Not_Null_Or_White_Space()
    {
        var result = ProductAttribute.Create(null, "key").Value.SetKey("new key");

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }
}