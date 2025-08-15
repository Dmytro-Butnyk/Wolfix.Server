using System.Net;
using Catalog.Domain.CategoryAggregate.Entities;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Catalog.Domain.CategoryAggregate;

public sealed class Category : BaseEntity
{
    public Category? Parent { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private readonly List<Guid> _productIds = [];
    public IReadOnlyCollection<Guid> ProductIds => _productIds.AsReadOnly();
    
    private readonly List<ProductVariant> _productVariants = [];
    public IReadOnlyCollection<ProductVariantInfo> ProductVariants => _productVariants
        .Select(pv => (ProductVariantInfo)pv)
        .ToList()
        .AsReadOnly();

    private readonly List<ProductAttribute> _productAttributes = [];
    public IReadOnlyCollection<ProductAttributeInfo> ProductAttributes => _productAttributes
        .Select(pa => (ProductAttributeInfo)pa)
        .ToList()
        .AsReadOnly();

    public int ProductsCount { get; private set; }
    private void RecalculateProductsCount()
    {
        ProductsCount = _productIds.Count;
    }
    
    private Category() { }

    private Category(string name, string? description = null, Category? parent = null)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public static Result<Category> Create(string name, string? description = null, Category? parent = null)
    {
        if (IsTextInvalid(name, out string nameErrorMessage))
        {
            return Result<Category>.Failure(nameErrorMessage);
        }
        
        if (description != null && IsTextInvalid(description, out string descriptionErrorMessage))
        {
            return Result<Category>.Failure(descriptionErrorMessage);
        }

        var category = new Category(name, description, parent)
        {
            ProductsCount = 0
        };

        return Result<Category>.Success(category, HttpStatusCode.Created);
    }

    public VoidResult ChangeName(string name)
    {
        if (IsTextInvalid(name, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Name = name;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeDescription(string? description)
    {
        if (description == " ")
        {
            return VoidResult.Failure($"{nameof(description)} cannot be white space");
        }
        
        Description = description;
        return VoidResult.Success();
    }
    
    #region productIds
    public VoidResult AddProductId(Guid productId)
    {
        if (IsGuidInvalid(productId, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }

        if (_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} already exists", HttpStatusCode.Conflict);
        }
        
        _productIds.Add(productId);
        RecalculateProductsCount();
        return VoidResult.Success();
    }

    public VoidResult RemoveProductId(Guid productId)
    {
        if (IsGuidInvalid(productId, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        if (!_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} does not exist", HttpStatusCode.Conflict);
        }
        
        _productIds.Remove(productId);
        RecalculateProductsCount();
        return VoidResult.Success();
    }

    public VoidResult RemoveAllProductIds()
    {
        _productIds.Clear();
        RecalculateProductsCount();
        return VoidResult.Success();
    }
    #endregion
    
    #region productVariant
    public Result<ProductVariantInfo> GetProductVariant(Guid productVariantId)
    {
        ProductVariant? productVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);

        if (productVariant == null)
        {
            return Result<ProductVariantInfo>.Failure($"{nameof(productVariant)} is null. Nothing to get.");
        }
        
        return Result<ProductVariantInfo>.Success((ProductVariantInfo)productVariant);
    }
    #endregion
    
    #region productVariants
    public VoidResult AddProductVariant(string key)
    {
        ProductVariant? existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Key == key);

        if (existingProductVariant != null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariant)} already exists", HttpStatusCode.Conflict);
        }
        
        Result<ProductVariant> createProductVariantResult = ProductVariant.Create(this, key);

        return createProductVariantResult.Map(
            onSuccess: productVariant =>
            {
                _productVariants.Add(productVariant);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductVariantResult.StatusCode)
        );
    }

    public VoidResult RemoveProductVariant(Guid productVariantId)
    {
        ProductVariant? existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
        
        if (existingProductVariant == null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariant)} does not exist", HttpStatusCode.NotFound);
        }
        
        _productVariants.Remove(existingProductVariant);
        return VoidResult.Success();
    }

    public VoidResult RemoveAllProductVariants()
    {
        _productVariants.Clear();
        return VoidResult.Success();
    }

    public VoidResult ChangeProductVariantKey(Guid productVariantId, string key)
    {
        ProductVariant? existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
        
        if (existingProductVariant == null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariant)} does not exist", HttpStatusCode.NotFound);
        }
        
        VoidResult setProductVariantKeyResult = existingProductVariant.SetKey(key);

        return setProductVariantKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantKeyResult.StatusCode)
        );
    }
    #endregion

    #region productAttribute
    public Result<ProductAttributeInfo> GetProductAttribute(Guid productAttributeId)
    {
        ProductAttribute? productAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);
        
        if (productAttribute == null)
        {
            return Result<ProductAttributeInfo>.Failure($"{nameof(productAttribute)} is null. Nothing to get.", HttpStatusCode.NotFound);
        }
        
        return Result<ProductAttributeInfo>.Success((ProductAttributeInfo)productAttribute);
    }
    #endregion
    
    #region productAttributes
    public VoidResult AddProductAttribute(string key)
    {
        ProductAttribute? existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Key == key);

        if (existingProductAttribute != null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttribute)} already exists", HttpStatusCode.Conflict);
        }
        
        Result<ProductAttribute> createProductAttributeResult = ProductAttribute.Create(this, key);

        return createProductAttributeResult.Map(
            onSuccess: productAttribute =>
            {
                _productAttributes.Add(productAttribute);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductAttributeResult.StatusCode)
        );
    }

    public VoidResult RemoveProductAttribute(Guid productAttributeId)
    {
        ProductAttribute? existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);
        
        if (existingProductAttribute == null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttribute)} does not exist", HttpStatusCode.NotFound);
        }
        
        _productAttributes.Remove(existingProductAttribute);
        return VoidResult.Success();
    }

    public VoidResult RemoveAllProductAttributes()
    {
        _productAttributes.Clear();
        return VoidResult.Success();
    }

    public VoidResult ChangeProductAttributeKey(Guid productAttributeId, string key)
    {
        ProductAttribute? existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);

        if (existingProductAttribute == null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttribute)} does not exist", HttpStatusCode.NotFound);
        }

        VoidResult setProductAttributeKeyResult = existingProductAttribute.SetKey(key);

        return setProductAttributeKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductAttributeKeyResult.StatusCode)
        );
    }
    #endregion
    
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
    
    private bool IsGuidInvalid(Guid guid, out string errorMessage)
    {
        if (guid == Guid.Empty)
        {
            errorMessage = $"{nameof(guid)} is required";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    #endregion
}