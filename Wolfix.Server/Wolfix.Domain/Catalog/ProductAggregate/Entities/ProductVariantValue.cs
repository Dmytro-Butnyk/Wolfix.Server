using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

internal sealed class ProductVariantValue : BaseEntity
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
        if (IsTextInvalid(key, out var keyErrorMessage))
        {
            return Result<ProductVariantValue>.Failure(keyErrorMessage);
        }
        
        if (IsTextInvalid(value, out var valueErrorMessage))
        {
            return Result<ProductVariantValue>.Failure(valueErrorMessage);
        }

        var productVariant = new ProductVariantValue(product, key, value);
        return Result<ProductVariantValue>.Success(productVariant, HttpStatusCode.Created);
    }

    internal VoidResult SetKey(string key)
    {
        if (IsTextInvalid(key, out var keyErrorMessage))
        {
            return VoidResult.Failure(keyErrorMessage);
        }
        
        Key = key;
        return VoidResult.Success();
    }
    
    internal VoidResult SetValue(string value)
    {
        if (IsTextInvalid(value, out var valueErrorMessage))
        {
            return VoidResult.Failure(valueErrorMessage);
        }
        
        Value = value;
        return VoidResult.Success();
    }
    
    #region validation
    private static bool IsTextInvalid(string text, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            errorMessage = $"{nameof(text)} is required";
            return true;
        }

        errorMessage = string.Empty;
        return false;
    }
    #endregion
}

public record ProductVariantValueInfo(Guid Id, string Key, string Value);