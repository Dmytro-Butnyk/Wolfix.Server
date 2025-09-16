using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.AdditionDtos;
using Catalog.Application.Dto.Product.FullDto;
using Catalog.Application.Dto.Product.Review;
using Catalog.Domain.ProductAggregate.Enums;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Catalog.Application.Interfaces;

public interface IProductService
{
    Task<VoidResult> AddProductAsync(AddProductDto addProductDto, CancellationToken ct);
    Task<VoidResult> ChangeProductMainPhotoAsync(Guid productId, Guid newMainPhotoId, CancellationToken ct);
    Task<VoidResult> AddProductMediaAsync(AddMediaDto addMediaDto, CancellationToken ct);
    Task<VoidResult> DeleteProductMediaAsync(Guid productId, Guid mediaId, CancellationToken ct);
    Task<Result<ProductFullDto>> GetProductFullInfo(Guid productId, CancellationToken ct);
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRandomProductsAsync(int pageSize, CancellationToken ct);
    
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageByCategoryIdAsync(Guid childCategoryId,
        int page, int pageSize, CancellationToken ct);
    
    Task<Result<PaginationDto<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize,
        CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<ProductShortDto>>> GetRecommendedForPageAsync(int pageSize,
        List<Guid> visitedCategoriesIds, CancellationToken ct);
    
    Task<Result<CursorPaginationDto<ProductReviewDto>>> GetReviewsAsync(Guid productId, int pageSize, Guid? lastId,
        CancellationToken ct);

    Task<VoidResult> AddReviewAsync(Guid productId, AddProductReview addProductReviewDto, CancellationToken ct);
}