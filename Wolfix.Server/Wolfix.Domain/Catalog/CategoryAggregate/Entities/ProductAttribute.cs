using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.CategoryAggregate.Entities;

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
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<ProductAttribute>.Failure($"{nameof(key)} is required");
        }

        var attribute = new ProductAttribute(category, key);
        return Result<ProductAttribute>.Success(attribute, HttpStatusCode.Created);
    }
}

public record ProductAttributeInfo(string Key);