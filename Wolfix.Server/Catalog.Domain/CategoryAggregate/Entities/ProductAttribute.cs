using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.CategoryAggregate.Entities;

internal sealed class ProductAttribute : BaseEntity
{
    public Category Category { get; private set; }
    public string Key { get; private set; }
    
    private ProductAttribute() { }
    
    private ProductAttribute(Category category, string key)
    {
        Category = category;
        Key = key;
    }

    internal static Result<ProductAttribute> Create(Category category, string key)
    {
        if (IsKeyInvalid(key))
        {
            return Result<ProductAttribute>.Failure($"{nameof(key)} is required");
        }

        var attribute = new ProductAttribute(category, key);
        return Result<ProductAttribute>.Success(attribute, HttpStatusCode.Created);
    }

    internal VoidResult SetKey(string key)
    {
        if (IsKeyInvalid(key))
        {
            return VoidResult.Failure($"{nameof(key)} is required");
        }
        
        Key = key;
        return VoidResult.Success();   
    }
    
    private static bool IsKeyInvalid(string key)
    {
        return string.IsNullOrWhiteSpace(key);
    } 
}

public record ProductAttributeInfo(string Key);