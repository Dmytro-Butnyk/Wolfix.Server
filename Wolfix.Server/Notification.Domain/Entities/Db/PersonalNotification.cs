using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Notification.Domain.Entities.Db;

public sealed class PersonalNotification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }
    
    private PersonalNotification()
    {
    }
    
    private PersonalNotification(Guid userId, string message, DateTime createdAt, bool isRead)
    {
        UserId = userId;
        Message = message;
        CreatedAt = createdAt;
        IsRead = isRead;
    }
    
    public static Result<PersonalNotification> Create(Guid userId, string message)
    {
        if (userId == Guid.Empty)
            return Result<PersonalNotification>.Failure("UserId cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(message))
            return Result<PersonalNotification>.Failure("Message cannot be empty.");
        
        DateTime createdAt = DateTime.UtcNow;
        bool isRead = false;
        
        PersonalNotification notification = new(userId, message, createdAt, isRead);
        
        return Result<PersonalNotification>.Success(notification);
    }
    
    public void MarkAsRead()
    {
        IsRead = true;
    }
}