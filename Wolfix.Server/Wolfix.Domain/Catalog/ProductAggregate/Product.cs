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
    
    private readonly List<Review> _reviews = []; //✅
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly(); //✅

    private readonly List<ProductAttributeValue> _productsAttributeValues = []; //✅
    public IReadOnlyCollection<ProductAttributeValue> ProductsAttributeValues => _productsAttributeValues.AsReadOnly(); //✅
    
    private readonly List<ProductVariantValue> _productVariantValues = []; //✅
    public IReadOnlyCollection<ProductVariantValue> ProductVariantValues => _productVariantValues.AsReadOnly(); //✅
    
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
    
    //todo: userId
    #region reviews
    public VoidResult AddReview(string title, string text, uint rating)
    {
        var createReviewResult = Review.Create(title, text, rating, this);

        return createReviewResult.Map(
            onSuccess: review =>
            {
                _reviews.Add(review);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createReviewResult.StatusCode)
        );
    }
    
    public VoidResult RemoveReview(Guid reviewId)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to remove.", HttpStatusCode.NotFound);
        }
        
        _reviews.Remove(review);
        return VoidResult.Success();
    }

    public VoidResult RemoveAllReviews()
    {
        _reviews.Clear();
        return VoidResult.Success();
    }

    public VoidResult ChangeReviewTitle(Guid reviewId, string title)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setReviewTitleResult = review.SetTitle(title);
        
        return setReviewTitleResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewTitleResult.StatusCode)
        );
    }

    public VoidResult ChangeReviewText(Guid reviewId, string text)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setReviewTextResult = review.SetText(text);
        
        return setReviewTextResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewTextResult.StatusCode)
        );
    }

    public VoidResult ChangeReviewRating(Guid reviewId, uint rating)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setReviewRatingResult = review.SetRating(rating);
        
        return setReviewRatingResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewRatingResult.StatusCode)
        );
    }
    #endregion

    #region productAttributeValues
    public VoidResult AddProductAttributeValue(string key, string value)
    {
        var existingProductAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Value == value);

        if (existingProductAttributeValue != null)
        {
            return VoidResult.Failure($"{nameof(existingProductAttributeValue)} already exists", HttpStatusCode.Conflict);
        }
        
        var createProductAttributeValueResult = ProductAttributeValue.Create(this, key, value);

        return createProductAttributeValueResult.Map(
            onSuccess: productAttributeValue =>
            {
                _productsAttributeValues.Add(productAttributeValue);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductAttributeValueResult.StatusCode)
        );
    }

    public VoidResult RemoveProductAttributeValue(Guid productAttributeValueId)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);
        
        if (productAttributeValue == null)
        {
            return VoidResult.Failure($"{nameof(productAttributeValue)} is null. Nothing to remove.", HttpStatusCode.NotFound);
        }
        
        _productsAttributeValues.Remove(productAttributeValue);
        return VoidResult.Success();
    }
    
    public VoidResult RemoveAllProductAttributeValues()
    {
        _productsAttributeValues.Clear();
        return VoidResult.Success();
    }

    public VoidResult ChangeProductAttributeKey(Guid productAttributeValueId, string key)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);
        
        if (productAttributeValue == null)
        {
            return VoidResult.Failure($"{nameof(productAttributeValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setProductAttributeKeyResult = productAttributeValue.SetKey(key);

        return setProductAttributeKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductAttributeKeyResult.StatusCode)
        );
    }

    public VoidResult ChangeProductAttributeValue(Guid productAttributeValueId, string value)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);
        
        if (productAttributeValue == null)
        {
            return VoidResult.Failure($"{nameof(productAttributeValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setProductAttributeValueResult = productAttributeValue.SetValue(value);

        return setProductAttributeValueResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductAttributeValueResult.StatusCode)
        );
    }
    #endregion
    
    #region productVariantValues
    public VoidResult AddProductVariantValue(string key, string value)
    {
        var existingProductVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Value == value);

        if (existingProductVariantValue != null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariantValue)} already exists", HttpStatusCode.Conflict);
        }
        
        var createProductVariantValueResult = ProductVariantValue.Create(this, key, value);

        return createProductVariantValueResult.Map(
            onSuccess: productVariantValue =>
            {
                _productVariantValues.Add(productVariantValue);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductVariantValueResult.StatusCode)
        );
    }

    public VoidResult RemoveProductVariantValue(Guid productVariantValueId)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return VoidResult.Failure($"{nameof(productVariantValue)} is null. Nothing to remove.", HttpStatusCode.NotFound);
        }
        
        _productVariantValues.Remove(productVariantValue);
        return VoidResult.Success();
    }

    public VoidResult RemoveAllProductVariantValues()
    {
        _productVariantValues.Clear();
        return VoidResult.Success();
    }
    
    public VoidResult ChangeProductVariantKey(Guid productVariantValueId, string key)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return VoidResult.Failure($"{nameof(productVariantValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setProductVariantKeyResult = productVariantValue.SetKey(key);

        return setProductVariantKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantKeyResult.StatusCode)
        );
    }

    public VoidResult ChangeProductVariantValue(Guid productVariantValueId, string value)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return VoidResult.Failure($"{nameof(productVariantValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        var setProductVariantValueResult = productVariantValue.SetValue(value);

        return setProductVariantValueResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantValueResult.StatusCode)
        );
    }
    #endregion
}