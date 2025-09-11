using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Seller.Application.Interfaces;

public interface ISellerService
{
    Task<Result<FullNameDto>> ChangeFullNameAsync(Guid sellerId, ChangeFullNameDto request, CancellationToken ct);
    
    Task<Result<string>> ChangePhoneNumberAsync(Guid sellerId, ChangePhoneNumberDto request, CancellationToken ct);
    
    Task<Result<AddressDto>> ChangeAddressAsync(Guid sellerId, ChangeAddressDto request, CancellationToken ct);
    
    Task<Result<string>> ChangeBirthDateAsync(Guid sellerId, ChangeBirthDateDto request, CancellationToken ct);
}