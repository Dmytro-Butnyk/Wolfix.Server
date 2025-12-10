using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Interfaces;

namespace Support.Infrastructure.Repositories;

internal sealed class SupportRepository(SupportContext context)
    : BaseRepository<SupportContext, Domain.Entities.Support>(context), ISupportRepository
{
    private readonly DbSet<Domain.Entities.Support> _supports = context.Set<Domain.Entities.Support>();
}