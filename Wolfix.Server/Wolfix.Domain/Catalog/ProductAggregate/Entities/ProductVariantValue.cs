using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

public sealed class ProductVariantValue : BaseEntity
{
    public Product Product { get; private set; }
    
    public string Key { get; private set; }
    
    public string Value { get; private set; }
    
    private ProductVariantValue() { }

    private ProductVariantValue(Product product, string key, string value)
    {
        Product = product;
        Key = key;
        Value = value;
    }

    internal static Result<ProductVariantValue> Create(Product product, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<ProductVariantValue>.Failure($"{nameof(key)} is required");
        }
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ProductVariantValue>.Failure($"{nameof(value)} is required");
        }

        var productVariant = new ProductVariantValue(product, key, value);
        return Result<ProductVariantValue>.Success(productVariant, HttpStatusCode.Created);
    }
}