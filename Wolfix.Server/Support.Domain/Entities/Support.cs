using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Support.Domain.Entities;

public sealed class Support : BaseEntity
{
    public FullName FullName { get; set; }
    
    public Guid AccountId { get; set; }
    
    
}