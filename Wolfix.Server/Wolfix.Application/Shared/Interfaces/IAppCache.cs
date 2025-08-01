namespace Wolfix.Application.Shared.Interfaces;

public interface IAppCache
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    
    void Remove(string key);
}