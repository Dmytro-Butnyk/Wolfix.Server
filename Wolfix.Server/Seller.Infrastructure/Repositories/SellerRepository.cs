using Seller.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Repositories;

public sealed class SellerRepository(SellerContext context)
    : BaseRepository<SellerContext, Domain.SellerAggregate.Seller>(context), ISellerRepository
{
    
}