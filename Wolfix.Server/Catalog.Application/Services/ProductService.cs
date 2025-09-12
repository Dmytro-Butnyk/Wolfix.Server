using System.Net;
using System.Reflection.Metadata;
using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.AdditionDtos;
using Catalog.Application.Dto.Product.Review;
using Catalog.Application.Interfaces;
using Catalog.Application.Mapping.Product;
using Catalog.Application.Mapping.Product.Review;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.DomainServices;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Catalog.Domain.ValueObjects.AddProduct;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Shared.Application.Dto;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.Services;

internal sealed class ProductService(
    IProductRepository productRepository,
    IProductDomainService productDomainService,
    IEventBus eventBus) : IProductService
{
    public async Task<VoidResult> AddProductAsync(
        AddProductDto addProductDto, CancellationToken ct)
    {
        ProductStatus productStatus;

        if (Enum.TryParse(addProductDto.Status, out ProductStatus status))
        {
            productStatus = status;
        }
        else
        {
            return VoidResult.Failure("Invalid status");
        }

        BlobResourceType blobResourceType;

        if (Enum.TryParse(addProductDto.ContentType, out BlobResourceType resourceType))
        {
            blobResourceType = resourceType;
        }
        else
        {
            return VoidResult.Failure("Invalid blob resource type");
        }

        Stream stream = addProductDto.Filestream.OpenReadStream();

        IReadOnlyCollection<AddAttributeValueObject> attributes = addProductDto.Attributes
            .Select(attr => new AddAttributeValueObject(attr.Id, attr.Value))
            .ToList();

        Result<Guid> result = await productDomainService.AddProductAsync(
            addProductDto.Title,
            addProductDto.Description,
            addProductDto.Price,
            productStatus,
            addProductDto.CategoryId,
            attributes,
            ct
        );

        if (!result.IsSuccess)
        {
            return VoidResult.Failure(result.ErrorMessage!, result.StatusCode);
        }

        VoidResult eventResult = await eventBus.PublishAsync(
            new ProductMediaAdded(
                result.Value,
                new MediaEventDto(blobResourceType, stream, true)),
            ct);

        if (!eventResult.IsSuccess)
        {
            return VoidResult.Failure(eventResult.ErrorMessage!, eventResult.StatusCode);
        }

        return VoidResult.Success();
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct)
    {
        //todo: добавить проверку на существование категории через доменный сервис(или событие) и кинуть нот фаунт если нету

        int totalCount = await productRepository.GetTotalCountByCategoryAsync(childCategoryId, ct);

        if (totalCount == 0)
        {
            PaginationDto<ProductShortDto> dto = new(1, 1, 0, new List<ProductShortDto>());
            return Result<PaginationDto<ProductShortDto>>.Success(dto);
        }

        IReadOnlyCollection<ProductShortProjection> productsByCategory =
            await productRepository.GetAllByCategoryIdForPageAsync(childCategoryId, page, pageSize, ct);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        List<ProductShortDto> productShortDtos = productsByCategory
            .Select(product => product.ToShortDto())
            .ToList();

        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);

        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct)
    {
        int totalCount = await productRepository.GetTotalCountWithDiscountAsync(ct);

        if (totalCount == 0)
        {
            PaginationDto<ProductShortDto> dto = new(1, 1, 0, new List<ProductShortDto>());
            return Result<PaginationDto<ProductShortDto>>.Success(dto);
        }

        IReadOnlyCollection<ProductShortProjection> productsWithDiscount =
            await productRepository.GetForPageWithDiscountAsync(page, pageSize, ct);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        List<ProductShortDto> productShortDtos = productsWithDiscount
            .Select(product => product.ToShortDto())
            .ToList();

        PaginationDto<ProductShortDto> paginationDto = new(page, totalPages, totalCount, productShortDtos);

        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct)
    {
        //todo: добавить проверку на существование категорий через доменный сервис(или событие) и кинуть нот фаунт если нету

        List<ProductShortProjection> recommendedProducts = new(pageSize);

        int productsByCategorySize = pageSize / visitedCategoriesIds.Count;
        int remainder = pageSize % visitedCategoriesIds.Count;

        for (var i = 0; i < visitedCategoriesIds.Count; ++i)
        {
            int count = productsByCategorySize + (i < remainder ? 1 : 0);
            Guid id = visitedCategoriesIds[i];

            IReadOnlyCollection<ProductShortProjection> recommendedByCategory =
                await productRepository.GetRecommendedByCategoryIdAsync(id, count, ct);
            recommendedProducts.AddRange(recommendedByCategory);
        }

        if (recommendedProducts.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(
                "Recommended products not found",
                HttpStatusCode.NotFound
            );
        }

        List<ProductShortDto> productShortDtos = recommendedProducts
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<CursorPaginationDto<ProductReviewDto>>> GetReviewsAsync(Guid productId, int pageSize,
        Guid? lastId, CancellationToken ct)
    {
        if (!await productRepository.IsExistAsync(productId, ct))
        {
            return Result<CursorPaginationDto<ProductReviewDto>>.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<ProductReviewProjection> productReviews = lastId switch
        {
            null => await productRepository.GetProductReviewsAsync(productId, pageSize, ct),
            _ => await productRepository.GetNextProductReviewsAsync(productId, pageSize, lastId.Value, ct)
        };

        Guid? nextCursor = productReviews.Count > 0 ? productReviews.Last().Id : null;

        List<ProductReviewDto> productReviewsDto = productReviews
            .Select(productReview => productReview.ToDto())
            .ToList();

        CursorPaginationDto<ProductReviewDto> cursorPaginationDto = new(productReviewsDto, nextCursor);

        return Result<CursorPaginationDto<ProductReviewDto>>.Success(cursorPaginationDto);
    }

    public async Task<VoidResult> AddReviewAsync(Guid productId, AddProductReview addProductReviewDto,
        CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct);

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult result = await eventBus.PublishAsync(new CheckCustomerExistsForAddingReview
        {
            CustomerId = addProductReviewDto.CustomerId
        }, ct);

        if (!result.IsSuccess)
        {
            return result;
        }

        product.AddReview(addProductReviewDto.Title, addProductReviewDto.Text,
            addProductReviewDto.Rating, addProductReviewDto.CustomerId);

        await productRepository.SaveChangesAsync(ct);

        return VoidResult.Success();
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRandomProductsAsync(int pageSize,
        CancellationToken ct)
    {
        int productCount = await productRepository.GetTotalCountAsync(ct);

        if (productCount == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Success([]);
        }

        var random = new Random();
        int randomSkip = random.Next(1, productCount);

        IReadOnlyCollection<ProductShortProjection> randomProducts =
            await productRepository.GetRandomAsync(randomSkip, pageSize, ct);

        if (randomProducts.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Success([]);
        }

        List<ProductShortDto> randomProductsDto = randomProducts
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(randomProductsDto);
    }
}