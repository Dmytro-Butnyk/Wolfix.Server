using Microsoft.EntityFrameworkCore;
using Seller.Domain.Interfaces;
using Seller.Domain.Projections.Seller;
using Seller.Domain.SellerAggregate.Entities;
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

    public async Task<int> GetTotalCountAsync(CancellationToken ct)
    {
        return await _sellers
            .AsNoTracking()
            .CountAsync(ct);
    }

    public async Task<IReadOnlyCollection<SellerForAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _sellers
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include("_sellerCategories")
            .Select(seller => new SellerForAdminProjection(
                seller.Id,
                seller.PhotoUrl,
                seller.FullName,
                seller.PhoneNumber.Value,
                seller.Address,
                seller.BirthDate.Value,
                EF.Property<List<SellerCategory>>(seller, "_sellerCategories")
                    .Select(sc => sc.Name)
                    .ToList()))
            .ToListAsync(ct);
    }
}