using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.CategoryAggregate.Entities;

internal sealed class ProductVariant : BaseEntity
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
        if (IsTextInvalid(key, out var errorMessage))
        {
            return Result<ProductVariant>.Failure(errorMessage);
        }

        var productVariant = new ProductVariant(category, key);
        return Result<ProductVariant>.Success(productVariant, HttpStatusCode.Created);
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
    
    public static explicit operator ProductVariantInfo(ProductVariant productVariant)
        => new(productVariant.Id, productVariant.Key);
}

public record ProductVariantInfo(Guid Id, string Key);