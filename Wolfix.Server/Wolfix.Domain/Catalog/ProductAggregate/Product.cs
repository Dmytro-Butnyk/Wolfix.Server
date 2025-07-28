using System.Net;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;
using Wolfix.Domain.Catalog.ProductAggregate.Enums;
using Wolfix.Domain.Shared;

namespace Wolfix.Domain.Catalog.ProductAggregate;

public sealed class Product : BaseEntity
{
    public string Title { get; private set; }
    
    public string Description { get; private set; }
    
    public decimal Price { get; private set; }
    
    public ProductStatus Status { get; private set; }
    
    internal Discount? Discount { get; set; }
    
    public decimal FinalPrice { get; private set; }
    private void RecalculateFinalPrice()
    {
        if (IsDiscountExists(out _) || IsDiscountExpired(out _))
        {
            FinalPrice = Price;
        }
        
        FinalPrice = Price * (100 - Discount!.Percent) / 100;
    }
    
    private const decimal BonusPercent = 0.01m;
    public uint Bonuses { get; private set; }
    private void RecalculateBonuses()
    {
        Bonuses = (uint)Math.Round(Price * BonusPercent);
    }

    public double? AverageRating { get; private set; }
    private void RecalculateAverageRating()
    {
        AverageRating = _reviews.Count == 0
            ? null
            : Math.Round(_reviews.Average(r => r.Rating), MidpointRounding.AwayFromZero);
    }
    
    public Guid CategoryId { get; private set; }
    
    //todo: seller ID (именно ID, потому что другой контекст)
    
    private readonly List<BlobResource> _resources = [];
    public IReadOnlyCollection<BlobResource> Resources => _resources.AsReadOnly();
    
    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<ReviewInfo> Reviews => _reviews
        .Select(r => new ReviewInfo(r.Id, r.Title, r.Text, r.Rating, r.CreatedAt))
        .ToList()
        .AsReadOnly();

    private readonly List<ProductAttributeValue> _productsAttributeValues = [];
    public IReadOnlyCollection<ProductAttributeValueInfo> ProductsAttributeValues => _productsAttributeValues
        .Select(pav => new ProductAttributeValueInfo(pav.Id, pav.Key, pav.Value))
        .ToList()
        .AsReadOnly();
    
    private readonly List<ProductVariantValue> _productVariantValues = [];
    public IReadOnlyCollection<ProductVariantValueInfo> ProductVariantValues => _productVariantValues
        .Select(pvv => new ProductVariantValueInfo(pvv.Id, pvv.Key, pvv.Value))
        .ToList()
        .AsReadOnly();
    
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
        if (IsTextInvalid(title, out var titleErrorMessage))
        {
            return Result<Product>.Failure(titleErrorMessage);
        }

        if (IsTextInvalid(description, out var descriptionErrorMessage))
        {
            return Result<Product>.Failure(descriptionErrorMessage);
        }

        if (IsPriceInvalid(price, out var priceErrorMessage))
        {
            return Result<Product>.Failure(priceErrorMessage);
        }

        if (IsStatusInvalid(status, out var statusErrorMessage))
        {
            return Result<Product>.Failure(statusErrorMessage);
        }

        if (IsGuidInvalid(categoryId, out var categoryIdErrorMessage))
        {
            return Result<Product>.Failure(categoryIdErrorMessage);
        }

        var product = new Product(title, description, price, status, categoryId);
        
        product.RecalculateBonuses();
        product.FinalPrice = price;
        
        return Result<Product>.Success(product, HttpStatusCode.Created);
    }

    public VoidResult ChangeTitle(string title)
    {
        if (IsTextInvalid(title, out var titleErrorMessage))
        {
            return VoidResult.Failure(titleErrorMessage);
        }
        
        Title = title;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeDescription(string description)
    {
        if (IsTextInvalid(description, out var descriptionErrorMessage))
        {
            return VoidResult.Failure(descriptionErrorMessage);
        }
        
        Description = description;
        return VoidResult.Success();
    }

    public VoidResult ChangePrice(decimal price)
    {
        if (IsPriceInvalid(price, out var priceErrorMessage))
        {
            return VoidResult.Failure(priceErrorMessage);
        }
        
        Price = price;
        
        RecalculateFinalPrice();
        RecalculateBonuses();
        
        return VoidResult.Success();
    }

    public VoidResult ChangeStatus(ProductStatus status)
    {
        if (IsStatusInvalid(status, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Status = status;
        return VoidResult.Success();
    }

    #region categoryId
    public VoidResult ChangeCategory(Guid categoryId)
    {
        if (IsGuidInvalid(categoryId, out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
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
    public Result<DiscountInfo> GetDiscount()
    {
        if (IsDiscountExists(out var errorMessage))
        {
            return Result<DiscountInfo>.Failure(errorMessage);
        }
        
        var discountInfo = new DiscountInfo(Discount!.Id, Discount.Percent, Discount.ExpirationDateTime, Discount.Status);
        return Result<DiscountInfo>.Success(discountInfo);
    }

    public Result<Guid> GetDiscountId()
    {
        if (IsDiscountExists(out var errorMessage))
        {
            return Result<Guid>.Failure(errorMessage);
        }
        
        return Result<Guid>.Success(Discount!.Id);
    }
    
    public Result<uint> GetDiscountPercent()
    {
        if (IsDiscountExists(out var discountInvalidErrorMessage))
        {
            return Result<uint>.Failure(discountInvalidErrorMessage);
        }
        
        //todo:?
        if (IsDiscountExpired(out var discountExpiredErrorMessage))
        {
            return Result<uint>.Failure(discountExpiredErrorMessage);
        }
        
        return Result<uint>.Success(Discount!.Percent);
    }

    public Result<DateTime> GetDiscountExpirationDateTime()
    {
        if (IsDiscountExists(out var discountInvalidErrorMessage))
        {
            return Result<DateTime>.Failure(discountInvalidErrorMessage);
        }
        
        //todo:?
        if (IsDiscountExpired(out var discountExpiredErrorMessage))
        {
            return Result<DateTime>.Failure(discountExpiredErrorMessage);
        }
        
        return Result<DateTime>.Success(Discount!.ExpirationDateTime);
    }

    public Result<DiscountStatus> GetDiscountStatus()
    {
        if (IsDiscountExists(out var errorMessage))
        {
            return Result<DiscountStatus>.Failure(errorMessage);
        }
        
        return Result<DiscountStatus>.Success(Discount!.Status);
    }
    
    public VoidResult AddDiscount(uint percent, DateTime expirationDateTime)
    {
        var createDiscountResult = Discount.Create(percent, expirationDateTime, this);

        return createDiscountResult.Map(
            onSuccess: discount =>
            {
                Discount = discount;
                RecalculateFinalPrice();
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createDiscountResult.StatusCode)
        );
    }

    public VoidResult RemoveDiscount()
    {
        if (IsDiscountExists(out var errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Discount = null;
        RecalculateFinalPrice();
        return VoidResult.Success();
    }

    public VoidResult MakeDiscountExpired()
    {
        if (IsDiscountExists(out var discountInvalidErrorMessage))
        {
            return VoidResult.Failure(discountInvalidErrorMessage);
        }

        var setStatusResult = Discount!.SetStatus(DiscountStatus.Expired);
        
        return setStatusResult.Map(
            onSuccess: () =>
            {
                RecalculateFinalPrice();
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setStatusResult.StatusCode)
        );
    }
    #endregion
    
    #region review
    public Result<ReviewInfo> GetReview(Guid reviewId)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return Result<ReviewInfo>.Failure($"{nameof(review)} is null. Nothing to get.");
        }
        
        var reviewInfo = new ReviewInfo(review.Id, review.Title, review.Text, review.Rating, review.CreatedAt);
        return Result<ReviewInfo>.Success(reviewInfo);
    }

    public Result<string> GetReviewTitle(Guid reviewId)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
        
        if (review == null)
        {
            return Result<string>.Failure($"{nameof(review)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(review.Title);
    }

    public Result<string> GetReviewText(Guid reviewId)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
        
        if (review == null)
        {
            return Result<string>.Failure($"{nameof(review)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(review.Text);
    }

    public Result<uint> GetReviewRating(Guid reviewId)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
        
        if (review == null)
        {
            return Result<uint>.Failure($"{nameof(review)} is null. Nothing to get.");
        }
        
        return Result<uint>.Success(review.Rating);
    }
    #endregion
    
    //todo: userId
    //todo: private method for get by id with null check
    #region reviews
    public VoidResult AddReview(string title, string text, uint rating)
    {
        var createReviewResult = Review.Create(title, text, rating, this);

        return createReviewResult.Map(
            onSuccess: review =>
            {
                _reviews.Add(review);
                RecalculateAverageRating();
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
        RecalculateAverageRating();
        return VoidResult.Success();
    }

    public VoidResult RemoveAllReviews()
    {
        _reviews.Clear();
        AverageRating = null;
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
            onSuccess: () =>
            {
                RecalculateAverageRating();
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewRatingResult.StatusCode)
        );
    }
    #endregion
    
    #region productAttributeValue
    public Result<ProductAttributeValueInfo> GetProductAttributeValue(Guid productAttributeValueId)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);

        if (productAttributeValue == null)
        {
            return Result<ProductAttributeValueInfo>.Failure($"{nameof(productAttributeValue)} is null. Nothing to get.");
        }
        
        var productAttributeValueInfo = new ProductAttributeValueInfo(productAttributeValue.Id, productAttributeValue.Key, productAttributeValue.Value);
        return Result<ProductAttributeValueInfo>.Success(productAttributeValueInfo);
    }

    public Result<string> GetProductAttributeValueKey(Guid productAttributeValueId)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);
        
        if (productAttributeValue == null)
        {
            return Result<string>.Failure($"{nameof(productAttributeValue)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productAttributeValue.Key);
    }

    public Result<string> GetProductAttributeValueValue(Guid productAttributeValueId)
    {
        var productAttributeValue = _productsAttributeValues.FirstOrDefault(pav => pav.Id == productAttributeValueId);
        
        if (productAttributeValue == null)
        {
            return Result<string>.Failure($"{nameof(productAttributeValue)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productAttributeValue.Value);
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
    
    #region productVariantValue
    public Result<ProductVariantValueInfo> GetProductVariantValue(Guid productVariantValueId)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);

        if (productVariantValue == null)
        {
            return Result<ProductVariantValueInfo>.Failure($"{nameof(productVariantValue)} is null. Nothing to get.");
        }
        
        var productVariantValueInfo = new ProductVariantValueInfo(productVariantValue.Id, productVariantValue.Key, productVariantValue.Value);
        return Result<ProductVariantValueInfo>.Success(productVariantValueInfo);
    }

    public Result<string> GetProductVariantValueKey(Guid productVariantValueId)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return Result<string>.Failure($"{nameof(productVariantValue)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productVariantValue.Key);
    }

    public Result<string> GetProductVariantValueValue(Guid productVariantValueId)
    {
        var productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return Result<string>.Failure($"{nameof(productVariantValue)} is null. Nothing to get.");
        }
        
        return Result<string>.Success(productVariantValue.Value);
    }
    #endregion
    
    //todo
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

    private static bool IsPriceInvalid(decimal price, out string errorMessage)
    {
        if (price < 0)
        {
            errorMessage = $"{nameof(price)} must be positive";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }

    private static bool IsStatusInvalid(ProductStatus status, out string errorMessage)
    {
        if (!Enum.IsDefined(status))
        {
            errorMessage = $"{nameof(status)} is invalid";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    
    private static bool IsGuidInvalid(Guid guid, out string errorMessage)
    {
        if (guid == Guid.Empty)
        {
            errorMessage = $"{nameof(guid)} is required";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    
    private bool IsDiscountExists(out string errorMessage)
    {
        if (Discount == null)
        {
            errorMessage = $"{nameof(Discount)} does not exist.";
            return true;
        }
        
        errorMessage = string.Empty;
        return false;
    }
    
    private bool IsDiscountExpired(out string errorMessage)
    {
        if (Discount!.Status == DiscountStatus.Expired)
        {
            errorMessage = $"{nameof(Discount)} is expired. Nothing to do.";
            return true;
        }

        errorMessage = string.Empty;
        return false;
    }
    #endregion
}