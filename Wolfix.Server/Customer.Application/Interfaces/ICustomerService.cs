using Customer.Application.Dto;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Shared.Domain.Models;

namespace Customer.Application.Interfaces;

public interface ICustomerService
{
    Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct);
    Task<Result<IReadOnlyCollection<FavoriteItemDto>>> GetFavoriteItemsAsync(Guid customerId, CancellationToken ct);
}