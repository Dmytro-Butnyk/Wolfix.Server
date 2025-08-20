using Customer.Application.Dto;
using Shared.Domain.Models;

namespace Customer.Application.Interfaces;

public interface ICustomerService
{
    Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct);
}