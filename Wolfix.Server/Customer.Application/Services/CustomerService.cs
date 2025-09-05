using System.Net;
using Customer.Application.Dto;
using Customer.Application.Dto.CartItem;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Customer.Application.Interfaces;
using Customer.Application.Mapping.CartItem;
using Customer.Application.Mapping.FavoriteItem;
using Customer.Domain.Interfaces;
using Customer.Domain.Projections;
using Customer.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.Services;

internal sealed class CustomerService(ICustomerRepository customerRepository, IEventBus eventBus) : ICustomerService
{
    public async Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct)
    {
        if (!await customerRepository.IsExistAsync(request.CustomerId, ct))
        {
            return VoidResult.Failure(
                $"Customer with id: {request.CustomerId} not found", 
                HttpStatusCode.NotFound
            );
        }

        VoidResult result = await eventBus.PublishAsync(new CheckProductExistsForAddingToFavorite
        {
            ProductId = request.ProductId,
            CustomerId = request.CustomerId
        }, ct);

        return result;
    }

    public async Task<VoidResult> AddProductToCartAsync(AddProductToCartDto request, CancellationToken ct)
    {
        if (!await customerRepository.IsExistAsync(request.CustomerId, ct))
        {
            return VoidResult.Failure(
                $"Customer with id: {request.CustomerId} not found", 
                HttpStatusCode.NotFound
            );
        }
        
        VoidResult result = await eventBus.PublishAsync(new CheckProductExistsForAddingToCart
        {
            ProductId = request.ProductId,
            CustomerId = request.CustomerId
        }, ct);
        
        return result;
    }

    public async Task<Result<IReadOnlyCollection<FavoriteItemDto>>> GetFavoriteItemsAsync(Guid customerId,
        CancellationToken ct)
    {
        if (!await customerRepository.IsExistAsync(customerId, ct))
        {
            return Result<IReadOnlyCollection<FavoriteItemDto>>.Failure(
                $"Customer with id: {customerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<FavoriteItemProjection> favoriteItems =
            await customerRepository.GetFavoriteItemsAsync(customerId, ct);

        List<FavoriteItemDto> favoriteItemsDto = favoriteItems
            .Select(fi => fi.ToDto())
            .ToList();
        
        return Result<IReadOnlyCollection<FavoriteItemDto>>.Success(favoriteItemsDto);
    }

    public async Task<Result<CustomerCartItemsDto>> GetCartItemsAsync(Guid customerId, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsNoTrackingAsync(customerId, ct);

        if (customer is null)
        {
            return Result<CustomerCartItemsDto>.Failure(
                $"Customer with id: {customerId} not found",
                HttpStatusCode.NotFound
            );
        }

        IReadOnlyCollection<CartItemProjection> cartItems =
            await customerRepository.GetCartItemsAsync(customerId, ct);

        IReadOnlyCollection<CartItemDto> cartItemsDto = cartItems
            .Select(ci => ci.ToDto())
            .ToList();

        CustomerCartItemsDto customerCartItemsDto = new()
        {
            Items = cartItemsDto,
            CustomerId = customerId,
            BonusesAmount = customer.BonusesAmount,
            TotalCartPriceWithoutBonuses = customer.TotalCartPriceWithoutBonuses
        };
        
        return Result<CustomerCartItemsDto>.Success(customerCartItemsDto);
    }
}