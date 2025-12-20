using Admin.Domain.AdminAggregate.Enums;
using Admin.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Admin.Infrastructure.Repositories;

internal sealed class AdminRepository(AdminContext context)
    : BaseRepository<AdminContext, Domain.AdminAggregate.Admin>(context), IAdminRepository
{
    private readonly DbSet<Domain.AdminAggregate.Admin> _admins = context.Admins;
    
    public async Task<Guid?> GetIdByAccountIdAsync(Guid accountId, bool isSuperAdmin, CancellationToken ct)
    {
        return await _admins
            .AsNoTracking()
            .Where(admin => admin.AccountId == accountId 
                    && (isSuperAdmin ? admin.Type == AdminType.Super : admin.Type == AdminType.Basic))
            .Select(admin => admin.Id)
            .FirstOrDefaultAsync(ct);
    }
}