namespace Wolfix.Application.Shared.Interfaces;

public interface IAppCache
{
    Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
       CancellationToken ct, TimeSpan? expiration = null);
    
    void Remove(string key);
}