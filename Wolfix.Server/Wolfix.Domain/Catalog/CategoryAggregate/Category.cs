using System.Net;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.CategoryAggregate;

public sealed class Category : BaseEntity
{
    public Category? Parent { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private readonly List<Guid> _productIds = [];
    public IReadOnlyCollection<Guid> ProductIds => _productIds.AsReadOnly();
    
    private readonly List<ProductVariant> _productVariants = [];
    public IReadOnlyCollection<ProductVariantInfo> ProductVariants => _productVariants
        .Select(pv => new ProductVariantInfo(pv.Key))
        .ToList()
        .AsReadOnly();

    private readonly List<ProductAttribute> _productAttributes = [];
    public IReadOnlyCollection<ProductAttributeInfo> ProductAttributes => _productAttributes
        .Select(pa => new ProductAttributeInfo(pa.Key))
        .ToList()
        .AsReadOnly();

    public int ProductsCount => _productIds.Count;
    
    private Category() { }

    private Category(string name, string? description = null, Category? parent = null)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public static Result<Category> Create(string name, string? description = null, Category? parent = null)
    {
        if (IsTextInvalid(name, out var nameErrorMessage))
        {
            return Result<Category>.Failure(nameErrorMessage);
        }
        
        if (description != null && IsTextInvalid(description, out var descriptionErrorMessage))
        {
            return Result<Category>.Failure(descriptionErrorMessage);
        }

        var category = new Category(name, description, parent);
        return Result<Category>.Success(category, HttpStatusCode.Created);
    }

    public VoidResult ChangeName(string name)
    {
        if (IsTextInvalid(name, out var errorMessage))
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
        if (IsGuidInvalid(productId, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }

        if (_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} already exists", HttpStatusCode.Conflict);
        }
        
        _productIds.Add(productId);
        return VoidResult.Success();
    }

    public VoidResult RemoveProductId(Guid productId)
    {
        if (IsGuidInvalid(productId, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        if (!_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} does not exist", HttpStatusCode.Conflict);
        }
        
        _productIds.Remove(productId);
        return VoidResult.Success();
    }

    public VoidResult RemoveAllProductIds()
    {
        _productIds.Clear();
        return VoidResult.Success();
    }
    #endregion
    
    #region productVariant
    public Result<ProductVariantInfo> GetProductVariant(Guid productVariantId)
    {
        var productVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);

        if (productVariant == null)
        {
            return Result<ProductVariantInfo>.Failure($"{nameof(productVariant)} is null. Nothing to get.");
        }
        
        var productVariantInfo = new ProductVariantInfo(productVariant.Key);
        return Result<ProductVariantInfo>.Success(productVariantInfo);
    }

    public Result<string> GetProductVariantKey(Guid productVariantId)
    {
        var productVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
        
        if (productVariant == null)
        {
            return Result<string>.Failure($"{nameof(productVariant)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productVariant.Key);
    }
    #endregion
    
    #region productVariants
    public VoidResult AddProductVariant(string key)
    {
        var existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Key == key);

        if (existingProductVariant != null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariant)} already exists", HttpStatusCode.Conflict);
        }
        
        var createProductVariantResult = ProductVariant.Create(this, key);

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
        var existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
        
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
        var existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Id == productVariantId);
        
        if (existingProductVariant == null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariant)} does not exist", HttpStatusCode.NotFound);
        }
        
        var setProductVariantKeyResult = existingProductVariant.SetKey(key);

        return setProductVariantKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantKeyResult.StatusCode)
        );
    }
    #endregion

    #region productAttribute
    public Result<ProductAttributeInfo> GetProductAttribute(Guid productAttributeId)
    {
        var productAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);
        
        if (productAttribute == null)
        {
            return Result<ProductAttributeInfo>.Failure($"{nameof(productAttribute)} is null. Nothing to get.");
        }
        
        var productAttributeInfo = new ProductAttributeInfo(productAttribute.Key);
        return Result<ProductAttributeInfo>.Success(productAttributeInfo);
    }

    public Result<string> GetProductAttributeKey(Guid productAttributeId)
    {
        var productAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);
        
        if (productAttribute == null)
        {
            return Result<string>.Failure($"{nameof(productAttribute)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productAttribute.Key);
    }
    #endregion
    
    #region productAttributes
    public VoidResult AddProductAttribute(string key)
    {
        var existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Key == key);

        if (existingProductAttribute != null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttribute)} already exists", HttpStatusCode.Conflict);
        }
        
        var createProductAttributeResult = ProductAttribute.Create(this, key);

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
        var existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);
        
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
        var existingProductAttribute = _productAttributes.FirstOrDefault(pa => pa.Id == productAttributeId);

        if (existingProductAttribute == null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttribute)} does not exist", HttpStatusCode.NotFound);
        }

        var setProductAttributeKeyResult = existingProductAttribute.SetKey(key);

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