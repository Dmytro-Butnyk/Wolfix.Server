using System.Net;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate.Entities;

public sealed class ProductsAttributes : BaseEntity
{
    public Product Product { get; private set; }
    public Guid AttributeId { get; private set; }
    public string Value { get; private set; }
    
    private ProductsAttributes() { }

    private ProductsAttributes(Product product, Guid attributeId, string value)
    {
        Product = product;
        AttributeId = attributeId;
        Value = value;
    }

    internal static Result<ProductsAttributes> Create(Product product, Guid attributeId, string value)
    {
        if (Guid.Empty == attributeId)
        {
            return Result<ProductsAttributes>.Failure($"{nameof(attributeId)} is required");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ProductsAttributes>.Failure($"{nameof(value)} is required");
        }

        var productsAttributes = new ProductsAttributes(product, attributeId, value);
        return Result<ProductsAttributes>.Success(productsAttributes, HttpStatusCode.Created);
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

    internal VoidResult SetAttributeId(Guid attributeId)
    {
        if (Guid.Empty == attributeId)
        {
            return VoidResult.Failure($"{nameof(attributeId)} is required");
        }
        
        AttributeId = attributeId;
        return VoidResult.Success();
    }
}