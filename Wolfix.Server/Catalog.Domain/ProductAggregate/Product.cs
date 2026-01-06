using System.Net;
using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ProductAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Catalog.Domain.ProductAggregate;

public sealed class Product : BaseEntity
{
    public string Title { get; private set; }
    
    public string Description { get; private set; }
    
    public decimal Price { get; private set; }
    
    public ProductStatus Status { get; private set; }
    
    internal Discount? Discount { get; set; }
    
    public decimal FinalPrice { get; private set; }
    public Guid SellerId { get; private set; }
    public string? MainPhotoUrl => _productMedias.FirstOrDefault(pm => pm.IsMain)?.MediaUrl;
    
    private void RecalculateFinalPrice()
    {
        if (IsDiscountExists(out _) || IsDiscountExpired(out _))
        {
            FinalPrice = Price;
            return;
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
    
    private readonly List<ProductMedia> _productMedias = [];
    public IReadOnlyCollection<ProductMediaInfo> ProductMedias => _productMedias
        .Select(pm => (ProductMediaInfo)pm)
        .ToList()
        .AsReadOnly();
    
    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<ReviewInfo> Reviews => _reviews
        .Select(r => (ReviewInfo)r)
        .ToList()
        .AsReadOnly();
    
    #region ProductAttributeValues
    private readonly List<ProductAttributeValue> _productAttributeValues = [];
    public IReadOnlyCollection<ProductAttributeValue> ProductAttributeValues => _productAttributeValues
        .AsReadOnly();
    #endregion
    
    private readonly List<ProductVariantValue> _productVariantValues = [];
    public IReadOnlyCollection<ProductVariantValueInfo> ProductVariantValues => _productVariantValues
        .Select(pvv => (ProductVariantValueInfo)pvv)
        .ToList()
        .AsReadOnly();
    
    private Product() { }
    
    private Product(string title, string description, decimal price, ProductStatus status, Guid categoryId, Guid sellerId)
    {
        Title = title;
        Description = description;
        Price = price;
        Status = status;
        CategoryId = categoryId;
        SellerId = sellerId;
    }

    public static Result<Product> Create(string title, string description, decimal price, ProductStatus status, Guid categoryId, Guid sellerId)
    {
        if (IsTextInvalid(title, out string titleErrorMessage))
        {
            return Result<Product>.Failure(titleErrorMessage);
        }

        if (IsTextInvalid(description, out string descriptionErrorMessage))
        {
            return Result<Product>.Failure(descriptionErrorMessage);
        }

        if (IsPriceInvalid(price, out string priceErrorMessage))
        {
            return Result<Product>.Failure(priceErrorMessage);
        }

        if (IsStatusInvalid(status, out string statusErrorMessage))
        {
            return Result<Product>.Failure(statusErrorMessage);
        }

        if (IsGuidInvalid(categoryId, out string categoryIdErrorMessage))
        {
            return Result<Product>.Failure(categoryIdErrorMessage);
        }

        if (IsGuidInvalid(sellerId, out string sellerIdErrorMessage))
        {
            return Result<Product>.Failure(sellerIdErrorMessage);
        }

        var product = new Product(title, description, price, status, categoryId, sellerId);
        
        product.RecalculateBonuses();
        product.FinalPrice = price;
        
        return Result<Product>.Success(product, HttpStatusCode.Created);
    }

    #region Setters
    public VoidResult ChangeTitle(string title)
    {
        if (IsTextInvalid(title, out string titleErrorMessage))
        {
            return VoidResult.Failure(titleErrorMessage);
        }
        
        Title = title;
        return VoidResult.Success();
    }
    
    public VoidResult ChangeDescription(string description)
    {
        if (IsTextInvalid(description, out string descriptionErrorMessage))
        {
            return VoidResult.Failure(descriptionErrorMessage);
        }
        
        Description = description;
        return VoidResult.Success();
    }

    public VoidResult ChangePrice(decimal price)
    {
        if (IsPriceInvalid(price, out string priceErrorMessage))
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
        if (IsStatusInvalid(status, out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Status = status;
        return VoidResult.Success();
    }
    #endregion
    
    #region categoryId
    public VoidResult ChangeCategory(Guid categoryId)
    {
        if (IsGuidInvalid(categoryId, out string errorMessage))
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
        if (IsDiscountExists(out string errorMessage))
        {
            return Result<DiscountInfo>.Failure(errorMessage);
        }
        
        return Result<DiscountInfo>.Success((DiscountInfo)Discount!);
    }
    
    public VoidResult AddDiscount(uint percent, DateTime expirationDateTime)
    {
        Result<Discount> createDiscountResult = Discount.Create(percent, expirationDateTime, this);

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
        if (IsDiscountExists(out string errorMessage))
        {
            return VoidResult.Failure(errorMessage);
        }
        
        Discount = null;
        RecalculateFinalPrice();
        return VoidResult.Success();
    }

    public VoidResult MakeDiscountExpired()
    {
        if (IsDiscountExists(out string discountInvalidErrorMessage))
        {
            return VoidResult.Failure(discountInvalidErrorMessage);
        }

        VoidResult setStatusResult = Discount!.SetStatus(DiscountStatus.Expired);
        
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

    #region productMedia
    public Result<ProductMediaInfo> GetMedia(Guid productMediaId)
    {
        ProductMedia? productMedia = _productMedias
            .FirstOrDefault(p => p.Id == productMediaId);

        if (productMedia == null)
        {
            return Result<ProductMediaInfo>.Failure($"{nameof(productMedia)} is null. Nothing to get.");
        }
        
        return Result<ProductMediaInfo>.Success((ProductMediaInfo)productMedia);
    }
    
    public Result<string> GetMainPhoto()
    {
        ProductMedia? productMedia = _productMedias
            .FirstOrDefault(p => p.IsMain);

        if (productMedia == null)
        {
            return Result<string>.Failure($"Main photo is not set.");
        }
        
        return Result<string>.Success(productMedia.MediaUrl);
    }
    public VoidResult ChangeMainPhoto(Guid productMediaId)
    {
        ProductMedia? newMainPhoto = _productMedias
            .FirstOrDefault(p => p.Id == productMediaId);

        if (newMainPhoto is null)
        {
            return VoidResult.Failure(
                $"{nameof(newMainPhoto)} is null. Nothing to change.", 
                HttpStatusCode.NotFound
            );
        }
        
        if (newMainPhoto.MediaType == BlobResourceType.Video)
        {
            return VoidResult.Failure(
                $"{nameof(newMainPhoto)} is video. Can not be main photo."
            );
        }

        ProductMedia? currentMainPhoto = _productMedias
            .FirstOrDefault(p => p.IsMain);

        if (currentMainPhoto?.Id == newMainPhoto.Id)
        {
            return VoidResult.Success();
        }

        if (currentMainPhoto is not null)
        {
            currentMainPhoto.SetIsMain(false);
        }

        newMainPhoto.SetIsMain(true);

        return VoidResult.Success();
    }

    #endregion
    
    #region productMedias
    public VoidResult AddProductMedia(
        Guid mediaId,
        BlobResourceType mediaType,
        string mediaUrl, bool isMain)
    {
        if (!_productMedias.Any(pm => pm.IsMain))
        {
            isMain = true;

            if (mediaType == BlobResourceType.Video)
            {
                return VoidResult.Failure(
                    $"{nameof(mediaType)} is video. Can not be main photo."
                );
            }
        }
        else if (isMain)
        {
            isMain = false;
        }
        
        if (IsGuidInvalid(mediaId, out string mediaIdErrorMessage))
        {
            return VoidResult.Failure(mediaIdErrorMessage);
        }

        if (string.IsNullOrWhiteSpace(mediaUrl))
        {
            return VoidResult.Failure($"{nameof(mediaUrl)} is required");
        }
        
        Result<ProductMedia> createProductMediaResult = ProductMedia.Create(this, mediaId, mediaType, mediaUrl, isMain);

        return createProductMediaResult.Map(
            onSuccess: productMedia =>
            {
                _productMedias.Add(productMedia);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductMediaResult.StatusCode)
        );
    }

    public Result<Guid> RemoveProductMedia(Guid mediaId)
    {
        ProductMedia? productMedia = _productMedias.FirstOrDefault(p => p.Id == mediaId);

        if (productMedia == null)
        {
            return Result<Guid>.Failure($"{nameof(productMedia)} is null. Nothing to remove.", HttpStatusCode.NotFound);
        }
        
        if (productMedia.IsMain)
        {
            return Result<Guid>.Failure($"{nameof(productMedia)} is main. Can not remove.");
        }
        
        Guid productMediaId = productMedia.MediaId;
        
        _productMedias.Remove(productMedia);
        
        return Result<Guid>.Success(productMediaId);
    }
    
    public void RemoveAllProductMedias()
    {
        _productMedias.Clear();
    }
    
    #endregion
    
    #region review
    public Result<ReviewInfo> GetReview(Guid reviewId)
    {
        Review? review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return Result<ReviewInfo>.Failure($"{nameof(review)} is null. Nothing to get.");
        }
        
        return Result<ReviewInfo>.Success((ReviewInfo)review);
    }

    #endregion
    
    #region reviews
    public VoidResult AddReview(string title, string text, uint rating, Guid customerId)
    {
        Result<Review> createReviewResult = Review.Create(title, text, rating, this, customerId);

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
        Review? review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to remove.", HttpStatusCode.NotFound);
        }
        
        _reviews.Remove(review);
        RecalculateAverageRating();
        return VoidResult.Success();
    }

    public void RemoveAllReviews()
    {
        _reviews.Clear();
        AverageRating = null;
    }

    public VoidResult ChangeReviewTitle(Guid reviewId, string title)
    {
        Review? review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        VoidResult setReviewTitleResult = review.SetTitle(title);
        
        return setReviewTitleResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewTitleResult.StatusCode)
        );
    }

    public VoidResult ChangeReviewText(Guid reviewId, string text)
    {
        Review? review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        VoidResult setReviewTextResult = review.SetText(text);
        
        return setReviewTextResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setReviewTextResult.StatusCode)
        );
    }

    public VoidResult ChangeReviewRating(Guid reviewId, uint rating)
    {
        Review? review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            return VoidResult.Failure($"{nameof(review)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        VoidResult setReviewRatingResult = review.SetRating(rating);
        
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
    
    #region productAttributeValues
    public VoidResult AddProductAttributeValue(string key, string value, Guid attributeId)
    {
        if (_productAttributeValues.Any(av => av.CategoryAttributeId == attributeId && av.Key == value))
        {
            return VoidResult.Failure("Attribute already exists");
        }
        
        Result<ProductAttributeValue> attributeResult = ProductAttributeValue.Create(attributeId, key, value);
        if (attributeResult.IsFailure)
        {
            return VoidResult.Failure(attributeResult.ErrorMessage!, attributeResult.StatusCode);
        }

        _productAttributeValues.Add(attributeResult.Value!);
        
        return VoidResult.Success();
    }

    public void RemoveProductAttributeValue(Guid attributeId)
    {
        _productAttributeValues.RemoveAll(av => av.CategoryAttributeId == attributeId);
    }
    
    public VoidResult RemoveAllProductAttributeValues()
    {
        _productAttributeValues.Clear();

        if (_productAttributeValues.Count != 0)
        {
            return VoidResult.Failure("Failed to remove all product attribute values.");
        }
        
        return VoidResult.Success();
    }

    public VoidResult ChangeProductAttributeValue(Guid attributeId, string newValue)
    {
        var existing = _productAttributeValues.FirstOrDefault(a => a.CategoryAttributeId == attributeId);
        if (existing is null)
        {
            return VoidResult.Failure("Attribute not found", HttpStatusCode.NotFound);
        }

        _productAttributeValues.Remove(existing);
        
        var newAttr = new ProductAttributeValue(existing.CategoryAttributeId, existing.Key, newValue);
        _productAttributeValues.Add(newAttr);
        
        return VoidResult.Success();
    }
    #endregion
    
    #region productVariantValue
    public Result<ProductVariantValueInfo> GetProductVariantValue(Guid productVariantValueId)
    {
        ProductVariantValue? productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);

        if (productVariantValue == null)
        {
            return Result<ProductVariantValueInfo>.Failure($"{nameof(productVariantValue)} is null. Nothing to get.");
        }
        
        return Result<ProductVariantValueInfo>.Success((ProductVariantValueInfo)productVariantValue);
    }
    #endregion
    
    //todo
    #region productVariantValues
    public VoidResult AddProductVariantValue(string key, string value, Guid productVariantId)
    {
        ProductVariantValue? existingProductVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Value == value);

        if (existingProductVariantValue != null)
        {
            return VoidResult.Failure($"{nameof(existingProductVariantValue)} already exists", HttpStatusCode.Conflict);
        }
        
        Result<ProductVariantValue> createProductVariantValueResult = ProductVariantValue.Create(this, key, value, productVariantId);

        return createProductVariantValueResult.Map(
            onSuccess: productVariantValue =>
            {
                _productVariantValues.Add(productVariantValue);
                return VoidResult.Success();
            },
            onFailure: errorMessage => VoidResult.Failure(errorMessage, createProductVariantValueResult.StatusCode)
        );
    }

    public void RemoveProductVariantValue(Guid productVariantId)
    {
        ProductVariantValue? productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.CategoryVariantId == productVariantId);
        
        if (productVariantValue == null)
        {
            return;
        }
        
        _productVariantValues.Remove(productVariantValue);
    }

    public VoidResult RemoveAllProductVariantValues()
    {
        _productVariantValues.Clear();
        return VoidResult.Success();
    }
    
    public VoidResult ChangeProductVariantKey(Guid productVariantValueId, string key)
    {
        ProductVariantValue? productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return VoidResult.Failure($"{nameof(productVariantValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        VoidResult setProductVariantKeyResult = productVariantValue.SetKey(key);

        return setProductVariantKeyResult.Map(
            onSuccess: () => VoidResult.Success(),
            onFailure: errorMessage => VoidResult.Failure(errorMessage, setProductVariantKeyResult.StatusCode)
        );
    }

    public VoidResult ChangeProductVariantValue(Guid productVariantValueId, string value)
    {
        ProductVariantValue? productVariantValue = _productVariantValues.FirstOrDefault(pvv => pvv.Id == productVariantValueId);
        
        if (productVariantValue == null)
        {
            return VoidResult.Failure($"{nameof(productVariantValue)} is null. Nothing to change.", HttpStatusCode.NotFound);
        }
        
        VoidResult setProductVariantValueResult = productVariantValue.SetValue(value);

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
        if (price <= 0)
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