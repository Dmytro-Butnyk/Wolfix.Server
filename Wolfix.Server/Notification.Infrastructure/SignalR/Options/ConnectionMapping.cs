using System.Collections.Concurrent;
using Notification.Domain.Entities;
using Notification.Infrastructure.Interfaces;

namespace Notification.Infrastructure.SignalR.Options;

public sealed class ConnectionMapping : IConnectionMapping
{
    // Быстрый поиск инфы по ID соединения (для OnDisconnected)
    private readonly ConcurrentDictionary<string, ConnectionInfo> _connections = new();

    // Группировка подключений по ProfileId (для отправки уведомлений)
    // Key: ProfileId, Value: HashSet of ConnectionIds
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _profileConnections = new();

    public void Add(string connectionId, ConnectionInfo info)
    {
        _connections[connectionId] = info;

        _profileConnections.AddOrUpdate(info.ProfileId, 
            _ => new HashSet<string> { connectionId }, 
            (_, set) => 
            {
                lock (set) { set.Add(connectionId); }
                return set;
            });
    }

    public void Remove(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var info))
        {
            if (_profileConnections.TryGetValue(info.ProfileId, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0)
                    {
                        // Если у профиля не осталось подключений — чистим словарь
                        _profileConnections.TryRemove(info.ProfileId, out _);
                    }
                }
            }
        }
    }

    public ConnectionInfo? GetInfo(string connectionId) => 
        _connections.TryGetValue(connectionId, out var info) ? info : null;

    public IEnumerable<string> GetConnectionsByProfile(Guid profileId) =>
        _profileConnections.TryGetValue(profileId, out var set) ? set.ToList() : Enumerable.Empty<string>();
        
    // Для аккаунта можно искать перебором или добавить третий словарь, если критична скорость
    public IEnumerable<string> GetConnectionsByAccount(string accountId) =>
        _connections.Values.Where(x => x.AccountId == accountId).Select(x => x.ConnectionId);
}
