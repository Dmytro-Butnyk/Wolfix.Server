using Notification.Domain.Entities;

namespace Notification.Infrastructure.Interfaces;

//TODO ЗАРЕГИСТРИРОВАТЬ В КОНТЕЙНЕРЕ
public interface IConnectionMapping
{
    void Add(string connectionId, ConnectionInfo info);
    void Remove(string connectionId);
    ConnectionInfo? GetInfo(string connectionId);
    IEnumerable<string> GetConnectionsByProfile(Guid profileId);
    IEnumerable<string> GetConnectionsByAccount(string accountId);
}