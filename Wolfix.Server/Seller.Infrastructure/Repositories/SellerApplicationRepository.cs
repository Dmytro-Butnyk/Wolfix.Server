using Microsoft.EntityFrameworkCore;
using Seller.Domain.Interfaces;
using Seller.Domain.Projections.SellerApplication;
using Seller.Domain.SellerApplicationAggregate;
using Seller.Domain.SellerApplicationAggregate.Enums;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Repositories;

internal sealed class SellerApplicationRepository(SellerContext context)
    : BaseRepository<SellerContext, SellerApplication>(context), ISellerApplicationRepository
{
    private readonly DbSet<SellerApplication> _sellerApplications = context.Set<SellerApplication>();
    
    public async Task<IReadOnlyCollection<SellerApplication>> GetByAccountIdAsNoTrackingAsync(Guid accountId, CancellationToken ct)
    {
        return await _sellerApplications
            .AsNoTracking()
            .Where(sa => sa.AccountId == accountId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<SellerApplicationProjection>> GetPendingApplicationsAsync(CancellationToken ct)
    {
        return await _sellerApplications
            .AsNoTracking()
            .Where(sa => sa.Status == SellerApplicationStatus.Pending)
            .Select(sa => new SellerApplicationProjection(sa.Id, sa.CategoryName, sa.DocumentUrl, sa.SellerProfileData))
            .ToListAsync(ct);
    }
}