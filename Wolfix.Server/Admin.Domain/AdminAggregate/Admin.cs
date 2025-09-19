using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Admin.Domain.AdminAggregate;

public sealed class Admin : BaseEntity
{
    public Guid AccountId { get; private set; }
    
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
}