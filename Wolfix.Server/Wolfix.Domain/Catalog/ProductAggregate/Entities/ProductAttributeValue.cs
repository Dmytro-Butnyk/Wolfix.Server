using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

public sealed class ProductAttributeValue : BaseEntity
{
    public Product Product { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }
    
    private ProductAttributeValue() { }

    private ProductAttributeValue(Product product, string key, string value)
    {
        Product = product;
        Key = key;
        Value = value;
    }

    internal static Result<ProductAttributeValue> Create(Product product, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<ProductAttributeValue>.Failure($"{nameof(key)} is required");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ProductAttributeValue>.Failure($"{nameof(value)} is required");
        }

        var productsAttributes = new ProductAttributeValue(product, key, value);
        return Result<ProductAttributeValue>.Success(productsAttributes, HttpStatusCode.Created);
    }

    internal VoidResult SetValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            return VoidResult.Failure($"{nameof(newValue)} is required");
        }
        
        Value = newValue;
        return VoidResult.Success();
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