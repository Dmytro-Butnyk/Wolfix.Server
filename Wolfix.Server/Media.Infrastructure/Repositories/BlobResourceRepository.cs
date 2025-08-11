using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Media.Infrastructure.Repositories;

public sealed class BlobResourceRepository(MediaContext context):
    BaseRepository<MediaContext, BlobResource>(context), IBlobResourceRepository
{
    
}