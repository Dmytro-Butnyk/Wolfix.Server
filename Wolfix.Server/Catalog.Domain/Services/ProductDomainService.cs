using System.Net;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ProductAggregate.ValueObjects;
using Catalog.Domain.ValueObjects.AddProduct;
using Catalog.Domain.ValueObjects.FullProductDto;
using Shared.Domain.Models;

namespace Catalog.Domain.Services;

public sealed class ProductDomainService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<Result<Product>> CreateProductWithAttributesAsync(
        string title,
        string description,
        decimal price,
        ProductStatus status,
        Guid categoryId,
        Guid sellerId,
        IReadOnlyCollection<AddAttributeValueObject> attributeDtos,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdWithProductAttributesAsNoTrackingAsync(categoryId, ct);

        if (category is null)
        {
            return Result<Product>.Failure("Category not found", HttpStatusCode.NotFound);
        }

        Result<Product> newProduct = Product.Create(
            title,
            description,
            price,
            status,
            categoryId,
            sellerId
        );

        if (!newProduct.IsSuccess)
        {
            return Result<Product>.Failure(newProduct);
        }

        VoidResult addProductAttributesResult =
            AddProductAttributes(newProduct.Value!, attributeDtos, category.ProductAttributes);

        if (addProductAttributesResult.IsFailure)
        {
            return Result<Product>.Failure(addProductAttributesResult);
        }

        return Result<Product>.Success(newProduct.Value!);
    }

    private VoidResult AddProductAttributes(Product newProduct,
        IReadOnlyCollection<AddAttributeValueObject> incomingAttributes,
        IReadOnlyCollection<ProductAttributeInfo> allowedAttributes)
    {
        var allowedAttributesDict = allowedAttributes
            .ToDictionary(a => a.Id);

        foreach (AddAttributeValueObject incomingAttribute in incomingAttributes)
        {
            if (!allowedAttributesDict.TryGetValue(incomingAttribute.CategoryAttributeId,
                    out ProductAttributeInfo? attributeInfo))
            {
                return VoidResult.Failure($"Attribute with id:{incomingAttribute.CategoryAttributeId} not found",
                    HttpStatusCode.NotFound);
            }

            VoidResult addAttributeResult =
                newProduct.AddProductAttributeValue(
                    incomingAttribute.CategoryAttributeId,
                    attributeInfo.Key,
                    incomingAttribute.Value);

            if (addAttributeResult.IsFailure)
            {
                return VoidResult.Failure(addAttributeResult);
            }
        }

        return VoidResult.Success();
    }

    public async Task<IReadOnlyCollection<Guid>> GetAllMediaIdsByCategoryProducts(Guid categoryId, CancellationToken ct)
    {
        return await productRepository.GetAllMediaIdsByCategoryProductsAsync(categoryId, ct);
    }

    public async Task<Result<IReadOnlyCollection<ProductCategoriesValueObject>>> GetCategoriesLineForProduct(
        Guid categoryId, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsNoTrackingAsync(categoryId, ct, "Parent");

        if (category is null)
        {
            return Result<IReadOnlyCollection<ProductCategoriesValueObject>>
                .Failure("Category not found", HttpStatusCode.NotFound);
        }

        if (!category.IsChild)
        {
            return Result<IReadOnlyCollection<ProductCategoriesValueObject>>
                .Failure("Category is not a child category");
        }

        IReadOnlyCollection<ProductCategoriesValueObject> categoriesValueObjects =
        [
            new()
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Order = 2
            },
            new()
            {
                CategoryId = category.Parent.Id,
                CategoryName = category.Parent.Name,
                Order = 1
            }
        ];

        return Result<IReadOnlyCollection<ProductCategoriesValueObject>>
            .Success(categoriesValueObjects);
    }

    public async Task<VoidResult> IsCategoryExistAsync(Guid categoryId, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsNoTrackingAsync(categoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure($"Category with id:{categoryId} not found", HttpStatusCode.NotFound);
        }

        return VoidResult.Success();
    }

    public async Task<VoidResult> IsCategoryAndAttributesExistAsync(Guid categoryId, List<Guid> attributeIds,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdWithProductAttributesAsNoTrackingAsync(categoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure($"Category with id:{categoryId} not found", HttpStatusCode.NotFound);
        }

        HashSet<Guid> categoryAttributeIds = category.ProductAttributes
            .Select(a => a.Id)
            .ToHashSet();

        bool allExist = attributeIds.All(id => categoryAttributeIds.Contains(id));

        if (!allExist)
        {
            return VoidResult.Failure("One or more attributes do not belong to this category");
        }

        return VoidResult.Success();
    }
}