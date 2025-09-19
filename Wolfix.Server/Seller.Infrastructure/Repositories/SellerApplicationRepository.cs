using Microsoft.EntityFrameworkCore;
using Seller.Domain.Interfaces;
using Seller.Domain.SellerApplicationAggregate;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Repositories;

internal sealed class SellerApplicationRepository(SellerContext context)
    : BaseRepository<SellerContext, SellerApplication>(context), ISellerApplicationRepository
{
    private readonly DbSet<SellerApplication> _sellerApplications = context.Set<SellerApplication>();
}