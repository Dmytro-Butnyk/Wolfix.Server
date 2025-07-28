using System.Net;
using Wolfix.Application.Catalog.Dto.Product;
using Wolfix.Application.Catalog.Interfaces;
using Wolfix.Application.Catalog.Mapping.Product;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Catalog.Projections.Product;
using Wolfix.Domain.Shared;

namespace Wolfix.Application.Catalog.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Result<IEnumerable<ProductShortDto>>> GetAllByCategoryIdAsync(Guid childCategoryId,
        CancellationToken ct)
    {
        List<ProductShortProjection> productsByCategory =
            (await productRepository.GetAllByCategoryIdAsNoTrackingAsync(childCategoryId, ct))
            .ToList();

        if (productsByCategory.Count == 0)
        {
            return Result<IEnumerable<ProductShortDto>>.Failure(
                $"Products by category: {childCategoryId} not found",
                HttpStatusCode.NotFound
            );
        }

        List<ProductShortDto> productShortDtos = productsByCategory
            .Select(product => product.ToShortDto())
            .ToList();
        
        return Result<IEnumerable<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IEnumerable<ProductShortDto>>> GetForPageWithDiscountAsync(int page, int pageSize, CancellationToken ct)
    {
        List<ProductShortProjection> productsWithDiscount =
            (await productRepository.GetForPageWithDiscountAsync(page, pageSize, ct))
            .ToList();

        if (productsWithDiscount.Count == 0)
        {
            return Result<IEnumerable<ProductShortDto>>.Failure(
                "Products with discount not found",    
                HttpStatusCode.NotFound
            );
        }
        
        List<ProductShortDto> productShortDtos = productsWithDiscount
            .Select(product => product.ToShortDto())
            .ToList();
        
        return Result<IEnumerable<ProductShortDto>>.Success(productShortDtos);
    }

    public async Task<Result<IEnumerable<ProductShortDto>>> GetRandomProducts(int pageSize, CancellationToken ct)
    {
        int productCount = await productRepository.GetTotalCountAsync(ct);

        if (productCount == 0)
        {
            return Result<IEnumerable<ProductShortDto>>.Failure(
                "Products list is empty",    
                HttpStatusCode.NotFound
            );
        }
        
        Random random = new Random();
        int randomSkip = random.Next(1, productCount);

        List<ProductShortProjection> products = (await productRepository
            .GetRandom(randomSkip, pageSize, ct))
            .ToList();

        if (products.Count == 0)
        {
            return Result<IEnumerable<ProductShortDto>>.Failure(
                "Products not found",    
                HttpStatusCode.NotFound
            );
        }
        
        List<ProductShortDto> randomProducts = products
            .Select(product => product.ToShortDto())
            .ToList();

        return Result<IEnumerable<ProductShortDto>>.Success(randomProducts);
    }
}