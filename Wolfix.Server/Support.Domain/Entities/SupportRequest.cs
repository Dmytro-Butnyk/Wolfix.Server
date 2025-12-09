using Shared.Domain.Entities;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Support.Domain.Entities;

public sealed class SupportRequest : BaseEntity
{
    public string Email { get; private set; }
    
    public FullName FullName { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }
    
    public BirthDate BirthDate { get; private set; }
    
    public Guid CustomerId { get; private set; }
    
    public string Title { get; private set; }
    
    public Guid? ProductId { get; private set; }
    
    public string Content { get; private set; }

    public bool IsProcessed { get; private set; } = false;

    public Support? ProcessedBy { get; private set; } = null;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public VoidResult Process(Support support)
    {
        if (IsProcessed)
        {
            return VoidResult.Failure("Request is already processed");
        }

        if (ProcessedBy != null)
        {
            return VoidResult.Failure("Request is already processed by another support");
        }
        
        IsProcessed = true;
        ProcessedBy = support;
        return VoidResult.Success();
    }
}