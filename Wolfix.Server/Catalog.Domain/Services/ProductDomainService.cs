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
    public async Task<VoidResult> AddProductAsync(
        string title,
        string description,
        decimal price,
        ProductStatus status,
        Guid categoryId,
        AddMediaValueObject media,
        IReadOnlyCollection<AddAttributeValueObject> attributes,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsync(categoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure("Category not found", HttpStatusCode.NotFound);
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
            return VoidResult.Failure(newProduct.ErrorMessage!, newProduct.StatusCode);
        }

        await productRepository.AddAsync(newProduct.Value!, ct);

        VoidResult addProductAttributesResult =
            await AddProductAttributes(newProduct.Value!, attributes, ct);


        return VoidResult.Success();
    }

    private async Task<VoidResult> AddProductAttributes(Product newProduct,
        IReadOnlyCollection<AddAttributeValueObject> addAttributesDtos,
        CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsync(newProduct.CategoryId, ct);

        if (category is null)
        {
            return VoidResult.Failure("Category not found", HttpStatusCode.NotFound);
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

            newProduct.AddProductAttributeValue(key, addAttributesDto.Value);
        }

        await productRepository.SaveChangesAsync(ct);

        if (!isAllSuccess)
        {
            return VoidResult.Failure("Error during one or many attributes creation");
        }

        return VoidResult.Success();
    }
}