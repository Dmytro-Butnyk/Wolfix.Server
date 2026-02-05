using System.Net;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate.ValueObjects;

public sealed record ProductAttributeValue
{
    public Guid CategoryAttributeId { get; init; }
    public string Key { get; init; }
    public string? Value { get; init; }

    private ProductAttributeValue() { }

    public ProductAttributeValue(Guid categoryAttributeId, string key, string? value)
    {
        CategoryAttributeId = categoryAttributeId;
        Key = key;
        Value = value;
    }
    
    public static Result<ProductAttributeValue> Create(Guid categoryAttributeId, string key, string? value)
    {
        if (categoryAttributeId == Guid.Empty)
            return Result<ProductAttributeValue>.Failure($"{nameof(categoryAttributeId)} is required");
            
        if (string.IsNullOrWhiteSpace(key))
            return Result<ProductAttributeValue>.Failure($"{nameof(key)} is required");

        return Result<ProductAttributeValue>.Success(new ProductAttributeValue(categoryAttributeId, key, value));
    }
}

