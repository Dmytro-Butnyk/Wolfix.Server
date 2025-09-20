using Seller.Application.Dto.SellerApplication;
using Shared.Domain.Models;

namespace Seller.Application.Interfaces;

public interface ISellerApplicationService
{
    Task<VoidResult> CreateAsync(Guid accountId, CreateSellerApplicationDto request, CancellationToken ct);
    
    Task<IReadOnlyCollection<SellerApplicationDto>> GetPendingApplicationsAsync(CancellationToken ct);
    
    Task<VoidResult> ApproveApplicationAsync(Guid sellerApplicationId, CancellationToken ct);
    
    Task<VoidResult> RejectApplicationAsync(Guid sellerApplicationId, CancellationToken ct);
}