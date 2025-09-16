using System.Net;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.DomainServices;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.ValueObjects.AddProduct;
using Catalog.Domain.ValueObjects.FullProductDto;
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
        //todo: убрать метод GetByIdWithProductAttributesAsNoTrackingAsync так как есть возможность добавлять инклуды как аргументы
        
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
            ProductAttributeInfo? attributeInfo = attributes.FirstOrDefault(a => a.Id == addAttributesDto.ProductAttributeId);

            if (attributeInfo is null)
            {
                isAllSuccess = false;
                continue;
            }

            string key = attributeInfo.Key;

            VoidResult addAttributeResult = newProduct.AddProductAttributeValue(key, addAttributesDto.Value, addAttributesDto.ProductAttributeId);

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
    
    public async Task<Result<IReadOnlyCollection<ProductCategoriesValueObject>>> GetCategoriesLineForProduct(Guid categoryId, CancellationToken ct)
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
}