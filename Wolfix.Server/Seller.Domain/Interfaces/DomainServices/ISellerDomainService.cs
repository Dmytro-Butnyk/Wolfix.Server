using Seller.Domain.SellerApplicationAggregate.ValueObjects;
using Shared.Domain.Models;

namespace Seller.Domain.Interfaces.DomainServices;

public interface ISellerDomainService
{
    Task<VoidResult> CreateSellerWithFirstCategoryAsync(Guid accountId, SellerProfileData sellerProfileData,
        Guid categoryId, string categoryName, CancellationToken ct);
}