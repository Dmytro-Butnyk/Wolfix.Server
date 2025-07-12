namespace Wolfix.Domain.Shared;

public sealed class BlobResource : BaseEntity
{
    public required string Url { get; set; }
    
    public required BlobResourceType Type { get; set; }
    
    public required BaseEntity Entity { get; set; }
}