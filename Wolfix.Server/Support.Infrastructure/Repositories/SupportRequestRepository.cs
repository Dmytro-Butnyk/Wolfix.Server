using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Support.Domain.Entities;
using Support.Domain.Interfaces;

namespace Support.Infrastructure.Repositories;

internal sealed class SupportRequestRepository(SupportContext context)
    : BaseRepository<SupportContext, SupportRequest>(context), ISupportRequestRepository
{
    private readonly DbSet<SupportRequest> _supportRequests = context.Set<SupportRequest>();
}