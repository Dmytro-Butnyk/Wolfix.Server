using Notification.Domain.Entities.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Notification.Domain.Entities.Db;

public sealed class BroadcastNotification : BaseEntity
{
    public NotificationType Type { get; private set; }
    public Guid? TargetGroupId { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpireAt { get; private set; }

    private BroadcastNotification()
    {
    }

    private BroadcastNotification(NotificationType type,
        Guid? targetGroupId,
        string message,
        DateTime createdAt,
        DateTime expireAt)
    {
        Type = type;
        TargetGroupId = targetGroupId;
        Message = message;
        CreatedAt = createdAt;
        ExpireAt = expireAt;
    }

    public static Result<BroadcastNotification> Create(
        NotificationType type,
        Guid? targetGroupId,
        string message,
        DateTime expireAt)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Result<BroadcastNotification>.Failure("Message cannot be empty.");
        }

        if (NotificationType.System == type)
        {
            targetGroupId = null;
        }

        DateTime createdAt = DateTime.UtcNow;

        if (expireAt <= createdAt)
            return Result<BroadcastNotification>.Failure("ExpireAt must be in the future.");

        BroadcastNotification notification = new(
            type, targetGroupId,
            message, createdAt, expireAt);

        return Result<BroadcastNotification>.Success(notification);
    }
}