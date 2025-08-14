using Shared.Domain.Enums;

namespace Shared.Domain.Entities;

public sealed class BlobResource : BaseEntity
{
    public string Url { get; private set; }
    
    public string BlobName { get; private set; }
    
    public BlobResourceType Type { get; private set; }
}