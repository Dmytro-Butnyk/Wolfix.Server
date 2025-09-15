using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.Entities;

internal sealed class ProductVariantValue : BaseEntity
{
    public Product Product { get; private set; }
    
    public string Key { get; private set; }
    
    public string? Value { get; private set; }
    
    public Guid CategoryVariantId { get; private set; }
    
    private ProductVariantValue() { }

    private ProductVariantValue(Product product, string key, string? value, Guid categoryVariantId)
    {
        Product = product;
        Key = key;
        Value = value;
        CategoryVariantId = categoryVariantId;
    }

    internal static Result<ProductVariantValue> Create(Product product, string key, string? value, Guid categoryVariantId)
    {
        if (IsTextInvalid(key, out var keyErrorMessage))
        {
            return Result<ProductVariantValue>.Failure(keyErrorMessage);
        }

        if (Guid.Empty == categoryVariantId)
        {
            return Result<ProductVariantValue>.Failure($"{nameof(categoryVariantId)} is required");
        }

        var productVariant = new ProductVariantValue(product, key, value, categoryVariantId);
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
    
    public static explicit operator ProductVariantValueInfo(ProductVariantValue productVariantValue)
        => new(productVariantValue.Id, productVariantValue.Key, productVariantValue.Value);
}

public record ProductVariantValueInfo(Guid Id, string Key, string Value);