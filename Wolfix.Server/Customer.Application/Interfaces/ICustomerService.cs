using Customer.Application.Dto;
using Customer.Application.Dto.CartItem;
using Customer.Application.Dto.Customer;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Shared.Domain.Models;

namespace Customer.Application.Interfaces;

public interface ICustomerService
{
    Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct);
    
    Task<VoidResult> AddProductToCartAsync(AddProductToCartDto request, CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<FavoriteItemDto>>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<Result<CustomerCartItemsDto>> GetCartItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<Result<FullNameDto>> ChangeFullName(Guid customerId, ChangeFullNameDto request, CancellationToken ct);
}