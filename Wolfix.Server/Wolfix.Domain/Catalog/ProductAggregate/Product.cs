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
    
    public Discount? Discount { get; private set; }
    
    //todo: public decimal? FinalPrice => Discount is { Status: DiscountStatus.Active } ? Price - Discount?.Percent / 100 * Price : Price;
    
    public Guid CategoryId { get; private set; }
    
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
    
    //todo: add blob resource, delete blob resource
    
    //todo: add product attribute, delete product attribute, change product attribute
    
    //todo: change attribute value, change attribute id
    
    //todo: add product variant, delete product variant
    
    //todo: add review, delete review
    
    //todo: change title, change description, change price
}