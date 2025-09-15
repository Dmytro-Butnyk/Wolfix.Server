using System.Net;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.DomainServices;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ValueObjects.AddProduct;
using Shared.Domain.Models;

namespace Catalog.Domain.Services;

public sealed class ProductDomainService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IProductDomainService
{
    public async Task<Result<Guid>> AddProductAsync(
        string title,
        string description,
        decimal price,
        ProductStatus status,
        Guid categoryId,
        IReadOnlyCollection<AddAttributeValueObject> attributes,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsNoTrackingAsync(categoryId, ct);

        if (category is null)
        {
            return Result<Guid>.Failure("Category not found", HttpStatusCode.NotFound);
        }

        Result<Product> newProduct = Product.Create(
            title,
            description,
            price,
            status,
            categoryId
        );

        if (!newProduct.IsSuccess)
        {
            return Result<Guid>.Failure(newProduct.ErrorMessage!, newProduct.StatusCode);
        }

        await productRepository.AddAsync(newProduct.Value!, ct);
        
        await productRepository.SaveChangesAsync(ct);
        
        //todo: придумать как обрабатывать ошибку, если случиться проблема с одним из атрибутов
        VoidResult addProductAttributesResult =
            await AddProductAttributesAsync(newProduct.Value!, attributes, ct);

        return Result<Guid>.Success(newProduct.Value!.Id);
    }
    
    private async Task<VoidResult> AddProductAttributesAsync(Product newProduct,
        IReadOnlyCollection<AddAttributeValueObject> addAttributesDtos,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdWithProductAttributesAsNoTrackingAsync(newProduct.CategoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure(
                $"Category with id: {newProduct.CategoryId} not found",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<ProductAttributeInfo> attributes = category.ProductAttributes;

        bool isAllSuccess = true;

        foreach (AddAttributeValueObject addAttributesDto in addAttributesDtos)
        {
            ProductAttributeInfo? attributeInfo = attributes.FirstOrDefault(a => a.Id == addAttributesDto.Id);

            if (attributeInfo is null)
            {
                isAllSuccess = false;
                continue;
            }

            string key = attributeInfo.Key;

            VoidResult addAttributeResult = newProduct.AddProductAttributeValue(key, addAttributesDto.Value);

            if (addAttributeResult.IsFailure)
            {
                isAllSuccess = false;
            }
        }

        await productRepository.SaveChangesAsync(ct);

        if (!isAllSuccess)
        {
            return VoidResult.Failure("Error during one or many attributes creation");
        }

        return VoidResult.Success();
    }

    public async Task<IReadOnlyCollection<Guid>> GetAllMediaIdsByCategoryProducts(Guid categoryId, CancellationToken ct)
    {
        return await productRepository.GetAllMediaIdsByCategoryProductsAsync(categoryId, ct);
    }

    public async Task<VoidResult> AddAttributeToProductsAsync(Guid childCategoryId, string key, CancellationToken ct)
    {
        IReadOnlyCollection<Product> productsByCategory = await productRepository.GetAllByCategoryAsync(childCategoryId, ct);
        
        if (productsByCategory.Count == 0)
        {
            return VoidResult.Success();
        }
        
        foreach (var product in productsByCategory)
        {
            VoidResult addAttributeToProductResult = product.AddProductAttributeValue(key, string.Empty);
            
            if (!addAttributeToProductResult.IsSuccess)
            {
                return VoidResult.Failure(addAttributeToProductResult);
            }
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> AddVariantToProductsAsync(Guid childCategoryId, string key, CancellationToken ct)
    {
        IReadOnlyCollection<Product> productsByCategory = await productRepository.GetAllByCategoryAsync(childCategoryId, ct);

        if (productsByCategory.Count == 0)
        {
            return VoidResult.Success();
        }
        
        foreach (var product in productsByCategory)
        {
            VoidResult addVariantResult = product.AddProductVariantValue(key, string.Empty);

            if (addVariantResult.IsFailure)
            {
                return VoidResult.Failure(addVariantResult);
            }
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteAttributeInProductsAsync(Guid childCategoryId, Guid attributeId, CancellationToken ct)
    {
        IReadOnlyCollection<Product> productsByCategory = await productRepository.GetAllByCategoryAsync(childCategoryId, ct);

        if (productsByCategory.Count == 0)
        {
            return VoidResult.Success();
        }
        
        foreach (var product in productsByCategory)
        {
            //todo: в метод продукта принимается айди аттрибутВелью а в метод сервиса приходит айди обычного аттрибута,
            // добавить в ProductAttributeValue и ProductVariantValue айди ключей без прямой привязки к ним,
            // и переделать методы RemoveProductAttributeValue и RemoveProductVariantValue,
            // и желательно вообще сущности переназвать, чтобы у категории был CategoryAttribute и CategoryVariant,
            // а у продукта ProductAttribute и ProductVariant
            VoidResult deleteAttributeResult = product.RemoveProductAttributeValue(attributeId);

            if (deleteAttributeResult.IsFailure)
            {
                return VoidResult.Failure(deleteAttributeResult);
            }
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteVariantInProductsAsync(Guid childCategoryId, Guid variantId, CancellationToken ct)
    {
        IReadOnlyCollection<Product> productsByCategory = await productRepository.GetAllByCategoryAsync(childCategoryId, ct);

        if (productsByCategory.Count == 0)
        {
            return VoidResult.Success();
        }
        
        foreach (var product in productsByCategory)
        {
            //todo: в метод продукта принимается айди аттрибутВелью а в метод сервиса приходит айди обычного аттрибута,
            // добавить в ProductAttributeValue и ProductVariantValue айди ключей без прямой привязки к ним,
            // и переделать методы RemoveProductAttributeValue и RemoveProductVariantValue,
            // и желательно вообще сущности переназвать, чтобы у категории был CategoryAttribute и CategoryVariant,
            // а у продукта ProductAttribute и ProductVariant
            VoidResult deleteVariantResult = product.RemoveProductVariantValue(variantId);

            if (deleteVariantResult.IsFailure)
            {
                return VoidResult.Failure(deleteVariantResult);
            }
        }
        
        return VoidResult.Success();
    }
}