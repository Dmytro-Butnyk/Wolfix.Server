namespace Wolfix.Domain.Shared;

public sealed class BlobResource : BaseEntity
{
    public string Url { get; private set; }
    
    public string BlobName { get; private set; }
    
    public BlobResourceType Type { get; private set; }
    
    // public required BaseEntity Entity { get; set; }
}