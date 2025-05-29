using BudgetFlow.Application.Common.Services.Abstract;
using Microsoft.Extensions.Caching.Memory;

namespace BudgetFlow.Application.Common.Services.Concrete;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration);

        _memoryCache.Set(key, value, cacheEntryOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }
}