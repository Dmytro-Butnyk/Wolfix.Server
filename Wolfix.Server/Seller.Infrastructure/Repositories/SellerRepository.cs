using Microsoft.EntityFrameworkCore;
using Seller.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Seller.Infrastructure.Repositories;

public sealed class SellerRepository(SellerContext context)
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
}