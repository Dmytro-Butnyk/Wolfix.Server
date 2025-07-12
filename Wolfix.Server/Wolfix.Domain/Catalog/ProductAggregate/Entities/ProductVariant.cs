using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

//todo
public sealed class ProductVariant : BaseEntity
{
    public Product Product { get; private set; }
    
    public string Key { get; private set; }
    
    public string Value { get; private set; }
    
    private ProductVariant() { }

    private ProductVariant(Product product, string key, string value)
    {
        Product = product;
        Key = key;
        Value = value;
    }

    internal static Result<ProductVariant> Create(Product product, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<ProductVariant>.Failure($"{nameof(key)} is required");
        }
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ProductVariant>.Failure($"{nameof(value)} is required");
        }

        var productVariant = new ProductVariant(product, key, value);
        return Result<ProductVariant>.Success(productVariant, HttpStatusCode.Created);
    }
}