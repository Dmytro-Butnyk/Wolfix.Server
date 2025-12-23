using Admin.Domain.AdminAggregate.Enums;
using Admin.Domain.Interfaces;
using Admin.Domain.Projections;
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

    public async Task<int> GetBasicAdminsTotalCountAsync(CancellationToken ct)
    {
        return await _admins
            .AsNoTracking()
            .Where(admin => admin.Type == AdminType.Basic)
            .CountAsync(ct);
    }

    public async Task<IReadOnlyCollection<BasicAdminProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _admins
            .AsNoTracking()
            .Where(admin => admin.Type == AdminType.Basic)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(admin => new BasicAdminProjection(
                admin.Id,
                admin.FullName,
                admin.PhoneNumber
            ))
            .ToListAsync(ct);
    }
}