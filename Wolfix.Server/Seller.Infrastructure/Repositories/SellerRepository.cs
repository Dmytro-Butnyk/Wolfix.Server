using Microsoft.EntityFrameworkCore;
using Seller.Domain.Interfaces;
using Seller.Domain.Projections;
using Seller.Domain.Projections.Seller;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Repositories;

internal sealed class SellerRepository(SellerContext context)
    : BaseRepository<SellerContext, Domain.SellerAggregate.Seller>(context), ISellerRepository
{
    private readonly DbSet<Domain.SellerAggregate.Seller> _sellers = context.Set<Domain.SellerAggregate.Seller>();
    
    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _sellers
            .AsNoTracking()
            .Where(s => s.AccountId == accountId)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<SellerProjection?> GetProfileInfoAsync(Guid sellerId, CancellationToken ct)
    {
        return await _sellers
            .AsNoTracking()
            .Where(s => s.Id == sellerId)
            .Select(s => new SellerProjection(
                s.Id,
                s.PhotoUrl,
                s.FullName,
                s.PhoneNumber.Value,
                s.Address,
                s.BirthDate.Value))
            .FirstOrDefaultAsync(ct);
    }
}