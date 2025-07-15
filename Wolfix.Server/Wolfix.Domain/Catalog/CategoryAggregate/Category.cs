using System.Net;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.CategoryAggregate;

public sealed class Category : BaseEntity
{
    public Category? Parent { get; private set; } //✅
    
    public string Name { get; private set; } //✅
    
    public string? Description { get; private set; } //✅
    
    private readonly List<Guid> _productIds = []; //✅
    public IReadOnlyCollection<Guid> ProductIds => _productIds.AsReadOnly(); //✅
    
    private readonly List<ProductVariant> _productVariants = []; //✅
    public IReadOnlyCollection<ProductVariant> ProductVariants => _productVariants.AsReadOnly(); //✅
    
    private Category() { }

    private Category(string name, string? description = null, Category? parent = null)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public static Result<Category> Create(string name, string? description = null, Category? parent = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Category>.Failure($"{nameof(name)} is required");
        }
        
        if (description != null && string.IsNullOrWhiteSpace(description))
        {
            return Result<Category>.Failure($"{nameof(description)} is required");
        }

        var category = new Category(name, description, parent);
        return Result<Category>.Success(category, HttpStatusCode.Created);
    }

    public VoidResult ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return VoidResult.Failure($"{nameof(name)} is required");
        }
        
        Name = name;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return VoidResult.Failure($"{nameof(description)} is required");
        }
        
        Description = description;
        return VoidResult.Success();
    }
    
    #region productIds
    public VoidResult AddProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            return VoidResult.Failure($"{nameof(productId)} is required");
        }

        if (_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} already exists");
        }
        
        _productIds.Add(productId);
        return VoidResult.Success();
    }

    public VoidResult RemoveProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            return VoidResult.Failure($"{nameof(productId)} is required");
        }
        
        if (!_productIds.Contains(productId))
        {
            return VoidResult.Failure($"{nameof(productId)} does not exist");
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
    
    #region productVariants
    public VoidResult AddProductVariant(string key)
    {
        var existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Key == key);

        if (existingProductVariant != null)
        {
            return VoidResult.Failure($"{nameof(key)} already exists");
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

    public VoidResult RemoveProductVariant(string key)
    {
        var existingProductVariant = _productVariants.FirstOrDefault(pv => pv.Key == key);
        
        if (existingProductVariant == null)
        {
            return VoidResult.Failure($"{nameof(key)} does not exist");
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
            return VoidResult.Failure($"{nameof(productVariantId)} does not exist");
        }
        
        var setProductVariantKeyResult = existingProductVariant.SetKey(key);

        return setProductVariantKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantKeyResult.StatusCode)
        );
    }
    #endregion
}