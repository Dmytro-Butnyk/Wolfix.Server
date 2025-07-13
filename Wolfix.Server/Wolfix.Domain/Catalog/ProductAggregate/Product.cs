using System.Net;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;
using Wolfix.Domain.Catalog.ProductAggregate.Enums;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate;

public sealed class Product : BaseEntity
{
    public string Title { get; private set; } //✅
    
    public string Description { get; private set; } //✅
    
    public decimal Price { get; private set; } //✅
    
    public ProductStatus Status { get; private set; } //✅
    
    public Discount? Discount { get; private set; } //✅
    
    public decimal FinalPrice //✅
    {
        get
        {
            if (Discount == null || Discount.Status == DiscountStatus.Expired)
            {
                return Price;
            }

            return Price * (100 - Discount.Percent) / 100;
        }
    }

    public Guid CategoryId { get; private set; } //✅
    
    //todo: seller ID (именно ID, потому что другой контекст)
    
    private readonly List<BlobResource> _resources = [];
    public IReadOnlyCollection<BlobResource> Resources => _resources.AsReadOnly();
    
    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private readonly List<ProductsAttributes> _productsAttributes = [];
    public IReadOnlyCollection<ProductsAttributes> ProductsAttributes => _productsAttributes.AsReadOnly();
    
    private readonly List<ProductVariant> _productVariants = [];
    public IReadOnlyCollection<ProductVariant> ProductVariants => _productVariants.AsReadOnly();
    
    private Product() { }
    
    private Product(string title, string description, decimal price, ProductStatus status, Guid categoryId)
    {
        Title = title;
        Description = description;
        Price = price;
        Status = status;
        CategoryId = categoryId;
    }

    public static Result<Product> Create(string title, string description, decimal price, ProductStatus status, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Product>.Failure($"{nameof(title)} is required");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Product>.Failure($"{nameof(description)} is required");
        }

        if (price < 0)
        {
            return Result<Product>.Failure($"{nameof(price)} must be positive");
        }

        if (!Enum.IsDefined(status))
        {
            return Result<Product>.Failure($"{nameof(status)} is invalid");
        }

        if (Guid.Empty == categoryId)
        {
            return Result<Product>.Failure($"{nameof(categoryId)} is required");
        }

        var product = new Product(title, description, price, status, categoryId);
        return Result<Product>.Success(product, HttpStatusCode.Created);
    }

    public VoidResult ChangeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return VoidResult.Failure($"{nameof(title)} is required");
        }
        
        Title = title;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return VoidResult.Failure($"{nameof(description)} is required");
        }
        
        Description = description;
        return VoidResult.Success();
    }

    public VoidResult ChangePrice(decimal price)
    {
        if (price < 0)
        {
            return VoidResult.Failure($"{nameof(price)} must be positive");
        }
        
        Price = price;
        return VoidResult.Success();
    }

    public VoidResult ChangeStatus(ProductStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            return VoidResult.Failure($"{nameof(status)} is invalid");
        }
        
        Status = status;
        return VoidResult.Success();
    }

    #region categoryId
    public VoidResult ChangeCategory(Guid categoryId)
    {
        if (Guid.Empty == categoryId)
        {
            return VoidResult.Failure($"{nameof(categoryId)} is required");
        }

        if (CategoryId == categoryId)
        {
            return VoidResult.Failure($"{nameof(categoryId)} is the same");
        }
        
        CategoryId = categoryId;
        return VoidResult.Success();
    }
    #endregion

    #region discount
    public VoidResult AddDiscount(uint percent, DateTime expirationDateTime)
    {
        var createDiscountResult = Discount.Create(percent, expirationDateTime);

        return createDiscountResult.Map(
            onSuccess: discount =>
            {
                Discount = discount;
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createDiscountResult.StatusCode)
        );
    }

    public VoidResult RemoveDiscount()
    {
        if (Discount == null)
        {
            return VoidResult.Failure($"{nameof(Discount)} is null. Nothing to remove.");
        }
        
        Discount = null;
        return VoidResult.Success();
    }

    public VoidResult MakeDiscountExpired()
    {
        if (Discount == null)
        {
            return VoidResult.Failure($"{nameof(Discount)} is null. Nothing to change.");
        }

        var setStatusResult = Discount.SetStatus(DiscountStatus.Expired);
        
        return setStatusResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setStatusResult.StatusCode)
        );
    }
    #endregion
}