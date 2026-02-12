using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.Entities;

internal sealed class ProductAttributeValue : BaseEntity
{
    public Product Product { get; private set; }
    public Guid ProductId { get; private set; }
    
    public string Key { get; private set; }
    
    public string? Value { get; private set; }
    
    public Guid CategoryAttributeId { get; private set; }
    
    private ProductAttributeValue() { }

    private ProductAttributeValue(Product product, string key, string? value, Guid categoryAttributeId)
    {
        Product = product;
        ProductId = product.Id;
        Key = key;
        Value = value;
        CategoryAttributeId = categoryAttributeId;
    }

    internal static Result<ProductAttributeValue> Create(Product product, string key, string? value, Guid categoryAttributeId)
    {
        if (IsTextInvalid(key, out var keyErrorMessage))
        {
            return Result<ProductAttributeValue>.Failure(keyErrorMessage);
        }

        if (Guid.Empty == categoryAttributeId)
        {
            return Result<ProductAttributeValue>.Failure($"{nameof(categoryAttributeId)} is required");
        }

        var productsAttributes = new ProductAttributeValue(product, key, value, categoryAttributeId);
        return Result<ProductAttributeValue>.Success(productsAttributes, HttpStatusCode.Created);
    }

    internal VoidResult SetValue(string newValue)
    {
        if (IsTextInvalid(newValue, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Value = newValue;
        return VoidResult.Success();
    }

    internal VoidResult SetKey(string key)
    {
        if (IsTextInvalid(key, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Key = key;
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
    
    public static explicit operator ProductAttributeValueInfo(ProductAttributeValue productAttributeValue)
        => new(productAttributeValue.Id, productAttributeValue.Key, productAttributeValue.Value, productAttributeValue.CategoryAttributeId);
}

public record ProductAttributeValueInfo(Guid Id, string Key, string Value, Guid CategoryAttributeId);