using System.Net;
using Customer.Application.Dto;
using Customer.Application.Interfaces;
using Customer.Domain.Interfaces;
using Shared.Domain.Models;

namespace Customer.Application.Services;

internal sealed class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<VoidResult> AddProductToFavoriteAsync(AddProductToFavoriteDto request, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, ct);

        if (customer is null)
        {
            return VoidResult.Failure("Customer not found", HttpStatusCode.NotFound);
        }

        VoidResult addToFavoriteResult = customer.AddFavoriteItem(request.Title, request.PhotoUrl, request.Price,
            request.Bonuses, request.AverageRating, request.FinalPrice);

        if (!addToFavoriteResult.IsSuccess)
        {
            return addToFavoriteResult;
        }
        
        return VoidResult.Success();
    }
}