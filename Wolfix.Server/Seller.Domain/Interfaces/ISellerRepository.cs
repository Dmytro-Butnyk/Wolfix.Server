using Shared.Domain.Interfaces;

namespace Seller.Domain.Interfaces;

public interface ISellerRepository : IBaseRepository<SellerAggregate.Seller>
{
    
}