using System.Net;
using Catalog.Application.Contracts;
using Catalog.Application.Dto.Discount.Requests;
using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.AdditionDtos;
using Catalog.Application.Dto.Product.AttributesFiltrationDto;
using Catalog.Application.Dto.Product.Change;
using Catalog.Application.Dto.Product.FullDto;
using Catalog.Application.Dto.Product.Review;
using Catalog.Application.Mapping.Product;
using Catalog.Application.Mapping.Product.Review;
using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Domain.Projections.Product;
using Catalog.Domain.Projections.Product.Review;
using Catalog.Domain.Services;
using Catalog.Domain.ValueObjects.AddProduct;
using Catalog.Domain.ValueObjects.FullProductDto;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Shared.Application.Dto;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    ProductDomainService productDomainService,
    EventBus eventBus,
    IToxicityService toxicityService)
{
    public async Task<VoidResult> CreateProductAsync(
        AddProductDto addProductDto, CancellationToken ct)
    {
        VoidResult checkSellerExistsResult = await eventBus.PublishWithoutResultAsync(
            new CheckSellerExistsForProductAddition(addProductDto.SellerId, addProductDto.CategoryId), ct);

        if (checkSellerExistsResult.IsFailure)
        {
            return VoidResult.Failure(checkSellerExistsResult);
        }
        
        if (!Enum.TryParse(addProductDto.Status, out ProductStatus productStatus))
        {
            return VoidResult.Failure("Invalid status");
        }

        if (!Enum.TryParse(addProductDto.ContentType, out BlobResourceType blobResourceType))
        {
            return VoidResult.Failure("Invalid blob resource type");
        }

        if (addProductDto.Media is null)
        {
            return VoidResult.Failure("Media is null");
        }

        IReadOnlyCollection<AddAttributeValueObject> attributes = addProductDto.Attributes
            .Select(attr => new AddAttributeValueObject(attr.Id, attr.Value))
            .ToList();

        Result<Product> result = await productDomainService.CreateProductWithAttributesAsync(
            addProductDto.Title,
            addProductDto.Description,
            addProductDto.Price,
            productStatus,
            addProductDto.CategoryId,
            addProductDto.SellerId,
            attributes,
            ct
        );

        if (result.IsFailure)
        {
            return VoidResult.Failure(result);
        }
        
        await productRepository.AddAsync(result.Value!, ct);
        await productRepository.SaveChangesAsync(ct);

        //todo: пофиксить (ажур аккаунт срок истёк)
        
        // VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
        //     new ProductMediaAdded(
        //         result.Value,
        //         new MediaEventDto(blobResourceType, addProductDto.Media, false)),
        //     ct);
        //
        // if (eventResult.IsFailure)
        // {
        //     return VoidResult.Failure(eventResult);
        // }

        return VoidResult.Success();
    }

    public async Task<VoidResult> ChangeProductMainPhotoAsync(Guid productId, Guid newMainPhotoId, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct, "_productMedias");

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult changeProductMainPhotoResult = product.ChangeMainPhoto(newMainPhotoId);

        if (changeProductMainPhotoResult.IsFailure)
        {
            return changeProductMainPhotoResult;
        }

        await productRepository.SaveChangesAsync(ct);

        return VoidResult.Success();
    }

    public async Task<VoidResult> AddProductMediaAsync(AddMediaDto addMediaDto, CancellationToken ct)
    {
        if (!Enum.TryParse(addMediaDto.ContentType, out BlobResourceType blobResourceType))
        {
            return VoidResult.Failure("Invalid blob resource type");
        }

        Product? product = await productRepository.GetByIdAsync(addMediaDto.ProductId, ct, "_productMedias");

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {addMediaDto.ProductId} not found",
                HttpStatusCode.NotFound
            );
        }

        int mediaCount = product.ProductMedias.Count;

        if (mediaCount >= 10)
        {
            return VoidResult.Failure("Product can not have more than 10 media");
        }

        if (addMediaDto.Media is null)
        {
            return VoidResult.Failure("Media is null");
        }

        VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
            new ProductMediaAdded(
                addMediaDto.ProductId,
                new MediaEventDto(blobResourceType, addMediaDto.Media, true)),
            ct);

        if (eventResult.IsFailure)
        {
            return eventResult;
        }

        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteProductMediaAsync(Guid productId, Guid mediaId, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct, "_productMedias");

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        Result<Guid> deleteProductMediaResult = product.RemoveProductMedia(mediaId);

        if (!deleteProductMediaResult.IsSuccess)
        {
            return VoidResult.Failure(deleteProductMediaResult);
        }

        await productRepository.SaveChangesAsync(ct);

        VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
            new ProductMediaDeleted(deleteProductMediaResult.Value), ct);

        if (!eventResult.IsSuccess)
        {
            return VoidResult.Failure(eventResult);
        }

        return VoidResult.Success();
    }

    public async Task<Result<ProductFullDto>> GetProductFullInfoAsync(Guid productId, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsNoTrackingAsync(productId, ct,
            "_productMedias", "_productVariantValues");

        if (product is null)
        {
            return Result<ProductFullDto>.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<ProductMediaDto> productMediasDto = product.ProductMedias.Select(pmi =>
            new ProductMediaDto()
            {
                Url = pmi.MediaUrl,
                ContentType = pmi.MediaType.ToString(),
                IsMain = pmi.IsMain
            }
        ).ToList();

        IReadOnlyCollection<ProductAttributeDto> productAttributesDto = product.ProductAttributeValues.Select(pav =>
            new ProductAttributeDto()
            {
                Key = pav.Key,
                Value = pav.Value,
            }
        ).ToList();
        
        IReadOnlyCollection<ProductVariantDto> productVariantsDto = product.ProductVariantValues.Select(pvv =>
            new ProductVariantDto()
            {
                Key = pvv.Key,
                Value = pvv.Value,
            }
        ).ToList();

        Result<IReadOnlyCollection<ProductCategoriesValueObject>> categoriesLineResult =
            await productDomainService.GetCategoriesLineForProduct(product.CategoryId, ct);

        if (categoriesLineResult.IsFailure)
        {
            return Result<ProductFullDto>.Failure(categoriesLineResult);
        }

        IReadOnlyCollection<ProductCategoryDto> categoriesLine =
            categoriesLineResult.Value!.Select(c => new ProductCategoryDto()
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Order = c.Order
                }
            ).ToList();

        Result<ProductSellerEventResult> fetchSellerInformationResult =
            await eventBus.PublishWithSingleResultAsync<FetchSellerInformation, ProductSellerEventResult>(
                new FetchSellerInformation(product.SellerId), ct);

        if (fetchSellerInformationResult.IsFailure)
        {
            return Result<ProductFullDto>.Failure(fetchSellerInformationResult);
        }

        ProductSellerDto sellerDto = new ProductSellerDto
        {
            SellerId = fetchSellerInformationResult.Value!.SellerId,
            SellerFullName = fetchSellerInformationResult.Value!.SellerFullName,
            SellerPhotoUrl = fetchSellerInformationResult.Value!.SellerPhotoUrl
        };

        ProductFullDto productFullDto = new()
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            FinalPrice = product.FinalPrice,
            Status = product.Status.ToString(),
            Bonuses = product.Bonuses,
            AverageRating = product.AverageRating,
            Categories = categoriesLine,
            Medias = productMediasDto,
            Attributes = productAttributesDto,
            Variants = productVariantsDto,
            Seller = sellerDto
        };

        return Result<ProductFullDto>.Success(productFullDto);
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct)
    {
        //todo: добавить проверку на существование категории через доменный сервис(или событие) и кинуть нот фаунт если нету

        int totalCount = await productRepository.GetTotalCountByCategoryAsync(childCategoryId, ct);

        if (totalCount == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Success(PaginationDto<ProductShortDto>.Empty(page));
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
            return Result<PaginationDto<ProductShortDto>>.Success(PaginationDto<ProductShortDto>.Empty(page));
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

    public async Task<VoidResult> AddReviewAsync(Guid productId, AddProductReviewDto request,
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

        VoidResult result = await eventBus.PublishWithoutResultAsync(new CheckCustomerExistsForAddingReview
        {
            CustomerId = request.CustomerId
        }, ct);

        if (!result.IsSuccess)
        {
            return result;
        }

        Result<bool> checkToxicityResult = await toxicityService.IsToxic(request.Text, ct);
        
        if (checkToxicityResult.IsFailure)
        {
            return VoidResult.Failure(checkToxicityResult);
        }
        
        bool isToxic = checkToxicityResult.Value;
        
        if (isToxic)
        {
            return VoidResult.Failure("Review is toxic");
        }

        VoidResult addProductReviewResult = product.AddReview(request.Title, request.Text, request.Rating, request.CustomerId);

        if (addProductReviewResult.IsFailure)
        {
            return addProductReviewResult;
        }

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

        IReadOnlyCollection<ProductShortProjection> randomProducts =
            await productRepository.GetRandomAsync(pageSize, ct);

        if (randomProducts.Count == 0)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Success([]);
        }

        List<ProductShortDto> randomProductsDto = randomProducts
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(randomProductsDto);
    }
    
    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetBySearchQueryAsync(
        string searchQuery,
        int pageSize,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure("Search query is null or empty");
        }
        
        IReadOnlyCollection<ProductShortProjection> productShortProjections = await productRepository
            .GetBySearchQueryAsync(searchQuery, pageSize, ct);

        IReadOnlyCollection<ProductShortDto> productShortDtos =
            productShortProjections.Select(p => p.ToShortDto()).ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetBySearchQueryAndCategoryAsync(
        Guid categoryId,
        string searchQuery,
        int pageSize,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure("Search query is null or empty");
        }
        
        VoidResult isCategoryExist = await productDomainService.IsCategoryExistAsync(categoryId, ct);
        
        if (isCategoryExist.IsFailure)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(isCategoryExist);
        }

        IReadOnlyCollection<ProductShortProjection> productShortProjections = await productRepository
            .GetBySearchQueryAndCategoryAsync(categoryId, searchQuery, pageSize, ct);

        IReadOnlyCollection<ProductShortDto> productShortDtos =
            productShortProjections.Select(p => p.ToShortDto()).ToList();

        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IReadOnlyCollection<ProductShortDto>>> GetByAttributesFiltrationAsync(AttributesFiltrationDto attributesFiltrationDto, int pageSize,
        CancellationToken ct)
    {
        VoidResult isCategoryAndtributesExist = await productDomainService
            .IsCategoryAndAttributesExistAsync(attributesFiltrationDto.CategoryId,
                attributesFiltrationDto.FiltrationAttribute.Select(fa => fa.AttributeId).ToList(), ct);
        
        if (isCategoryAndtributesExist.IsFailure)
        {
            return Result<IReadOnlyCollection<ProductShortDto>>.Failure(isCategoryAndtributesExist);
        }

        IReadOnlyCollection<Guid> productIds =
            await productRepository.GetByAttributesFiltrationAsNoTrackingAsync(
                attributesFiltrationDto.FiltrationAttribute
                    .Select(fa => (fa.AttributeId, fa.Value))
                    .ToList(),
                pageSize,
                ct);

        IReadOnlyCollection<ProductShortProjection> productShortProjections =
            await productRepository.GetShortProductsByIdsAsNoTrackingAsync(productIds, ct);
        
        IReadOnlyCollection<ProductShortDto> productShortDtos = productShortProjections
            .Select(p => p.ToShortDto())
            .ToList();
        
        return Result<IReadOnlyCollection<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<PaginationDto<ProductShortDto>>> GetAllBySellerCategoryForPageAsync(Guid sellerId,
        Guid categoryId, int page, int pageSize, CancellationToken ct)
    {
        VoidResult checkCategoryExistResult = await productDomainService.IsCategoryExistAsync(categoryId, ct);

        if (checkCategoryExistResult.IsFailure)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(checkCategoryExistResult);
        }
        
        VoidResult checkSellerExistResult = await eventBus.PublishWithoutResultAsync(new CheckSellerWithCategoryExist(sellerId, categoryId), ct);

        if (checkSellerExistResult.IsFailure)
        {
            return Result<PaginationDto<ProductShortDto>>.Failure(checkSellerExistResult);
        }
        
        int totalCount = await productRepository.GetTotalCountBySellerCategoryAsync(sellerId, categoryId, ct);
        
        IReadOnlyCollection<ProductShortProjection> projections =
            await productRepository.GetAllBySellerCategoryForPageAsync(sellerId, categoryId, page, pageSize, ct);

        if (projections.Count == 0)
        {
            return Result<PaginationDto<ProductShortDto>>.Success(PaginationDto<ProductShortDto>.Empty(page));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        IReadOnlyCollection<ProductShortDto> dto = projections
            .Select(projection => projection.ToShortDto())
            .ToList();

        PaginationDto<ProductShortDto> paginationDto = new(
            page,
            totalPages,
            totalCount,
            dto
        );
        
        return Result<PaginationDto<ProductShortDto>>.Success(paginationDto);
    }

    public async Task<VoidResult> AddDiscountAsync(Guid productId, AddDiscountDto addDiscountDto, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct, "Discount");

        if (product is null)
        {
            return VoidResult.Failure(
                $"product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult addDiscountResult = product.AddDiscount(addDiscountDto.Percent, addDiscountDto.ExpirationDateTime);

        if (addDiscountResult.IsFailure)
        {
            return addDiscountResult;
        }
        
        await productRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteDiscountAsync(Guid productId, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct, "Discount");

        if (product is null)
        {
            return VoidResult.Failure(
                $"product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult deleteDiscountResult = product.RemoveDiscount();

        if (deleteDiscountResult.IsFailure)
        {
            return deleteDiscountResult;
        }
        
        await productRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> ChangeProductGeneralInfoAsync(Guid productId, ChangeProductGeneralInfoDto request, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct);

        if (product is null)
        {
            return VoidResult.Failure(
                $"product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult changeTitleResult = product.ChangeTitle(request.Title);

        if (changeTitleResult.IsFailure)
        {
            return changeTitleResult;
        }

        VoidResult changeDescriptionResult = product.ChangeDescription(request.Description);

        if (changeDescriptionResult.IsFailure)
        {
            return changeDescriptionResult;
        }

        VoidResult changeCategoryResult = product.ChangeCategory(request.CategoryId);

        if (changeCategoryResult.IsFailure)
        {
            return changeCategoryResult;
        }
        
        await productRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> ChangeProductPrice(Guid productId, ChangeProductPriceDto request, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct, "Discount");

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult changePriceResult = product.ChangePrice(request.Price);

        if (changePriceResult.IsFailure)
        {
            return changePriceResult;
        }
        
        await productRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteProductAsync(Guid productId, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(productId, ct);

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {productId} not found",
                HttpStatusCode.NotFound
            );
        }

        var @event = new DeleteProductMedia
        {
            MediaUrl = product.MainPhotoUrl
        };
        
        //todo: пофиксить (ажур аккаунт срок истёк)
        
        // VoidResult deleteProductMediaResult = await eventBus.PublishWithoutResultAsync(@event, ct);
        //
        // if (deleteProductMediaResult.IsFailure)
        // {
        //     return deleteProductMediaResult;
        // }
        
        productRepository.Delete(product, ct);
        await productRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}