using HospitalSaaS.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HospitalSaaS.Infrastructure.Caching;

public sealed class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken ct) =>
        Task.FromResult(cache.TryGetValue<T>(key, out var value) ? value : default);

    public Task SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken ct)
    {
        cache.Set(key, value, duration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
