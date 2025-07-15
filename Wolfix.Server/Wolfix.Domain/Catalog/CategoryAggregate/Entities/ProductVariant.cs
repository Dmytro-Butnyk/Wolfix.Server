using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.CategoryAggregate.Entities;

public sealed class ProductVariant : BaseEntity
{
    public Category Category { get; private set; }
    
    public string Key { get; private set; }
    
    private ProductVariant() { }
    
    private ProductVariant(Category category, string key)
    {
        Category = category;
        Key = key;
    }

    internal static Result<ProductVariant> Create(Category category, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<ProductVariant>.Failure($"{nameof(key)} is required");
        }

        var productVariant = new ProductVariant(category, key);
        return Result<ProductVariant>.Success(productVariant, HttpStatusCode.Created);
    }

    internal VoidResult SetKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return VoidResult.Failure($"{nameof(key)} is required");
        }
        
        Key = key;
        return VoidResult.Success();
    }
}