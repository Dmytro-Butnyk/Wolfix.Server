using Admin.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Admin.Infrastructure.Repositories;

internal sealed class AdminRepository(AdminContext context)
    : BaseRepository<AdminContext, Domain.AdminAggregate.Admin>(context), IAdminRepository
{
    private readonly DbSet<Domain.AdminAggregate.Admin> _admins = context.Admins;
    
    
}