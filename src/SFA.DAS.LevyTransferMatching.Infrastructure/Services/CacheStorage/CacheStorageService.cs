using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;

public class CacheStorageService(IDistributedCache distributedCache) : ICacheStorageService
{
    public async Task SaveToCache<T>(string key, T item, int expirationInHours)
    {
        var json = JsonConvert.SerializeObject(item);

        await distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
        });
    }

    public async Task<T> RetrieveFromCache<T>(string key)
    {
        var json = await distributedCache.GetStringAsync(key);
        return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
    }

    public async Task DeleteFromCache(string key)
    {
        await distributedCache.RemoveAsync(key);
    }
}