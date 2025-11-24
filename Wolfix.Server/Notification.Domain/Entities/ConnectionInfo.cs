namespace Notification.Domain.Entities;

public record ConnectionInfo(
    string AccountId, 
    Guid ProfileId, 
    string Role, 
    string ConnectionId
);