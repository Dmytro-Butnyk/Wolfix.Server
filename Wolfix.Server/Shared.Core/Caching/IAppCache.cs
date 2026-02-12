namespace Shared.Core.Caching;

public interface IAppCache
{
    Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
       CancellationToken ct, TimeSpan? expiration = null, TimeSpan? slidingExpiration = null);
    
    void Remove(string key);
}