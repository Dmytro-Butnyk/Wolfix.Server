using Customer.Application.Dto;
using Customer.Application.Dto.CartItem;
using Customer.Application.Dto.Customer;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Customer.Application.Interfaces;

public interface ICustomerService
{
    Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct);
    
    Task<VoidResult> AddProductToCartAsync(AddProductToCartDto request, CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<FavoriteItemDto>>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<Result<CustomerCartItemsDto>> GetCartItemsAsync(Guid customerId, CancellationToken ct);
    
    Task<Result<FullNameDto>> ChangeFullNameAsync(Guid customerId, ChangeFullNameDto request, CancellationToken ct);
    
    Task<Result<string>> ChangePhoneNumberAsync(Guid customerId, ChangePhoneNumberDto request, CancellationToken ct);
    
    Task<Result<AddressDto>> ChangeAddressAsync(Guid customerId, ChangeAddressDto request, CancellationToken ct);
    
    Task<Result<string>> ChangeBirthDateAsync(Guid customerId, ChangeBirthDateDto request, CancellationToken ct);
    
    Task<Result<CustomerDto>> GetProfileInfoAsync(Guid customerId, CancellationToken ct);
    
    Task<VoidResult> DeleteFavoriteItemAsync(Guid customerId, Guid favoriteItemId, CancellationToken ct);
    
    Task<VoidResult> DeleteCartItemAsync(Guid customerId, Guid cartItemId, CancellationToken ct);
}