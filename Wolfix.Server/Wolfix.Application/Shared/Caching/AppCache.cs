using Microsoft.Extensions.Caching.Memory;
using Wolfix.Application.Shared.Interfaces;

namespace Wolfix.Application.Shared.Caching;

internal sealed class AppCache(IMemoryCache memoryCache) : IAppCache
{
    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        CancellationToken ct, TimeSpan? expiration = null)
    {
        if (memoryCache.TryGetValue(key, out T cacheEntry))
        {
            return cacheEntry;
        }

        T result = await factory(ct);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        memoryCache.Set(key, result, options);

        return result;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }
}