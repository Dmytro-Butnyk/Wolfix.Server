using Admin.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Admin.Infrastructure.Repositories;

internal sealed class AdminRepository(AdminContext context)
    : BaseRepository<AdminContext, Domain.AdminAggregate.Admin>(context), IAdminRepository
{
    private readonly DbSet<Domain.AdminAggregate.Admin> _admins = context.Admins;
    
    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _admins
            .AsNoTracking()
            .Where(admin => admin.AccountId == accountId)
            .Select(admin => admin.Id)
            .FirstOrDefaultAsync(ct);
    }
}