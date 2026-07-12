namespace Ambev.DeveloperEvaluation.Common.Caching;

/// <summary>
/// Cross-cutting cache abstraction used by Application query/command handlers.
/// The Redis-backed implementation is registered in <c>InfrastructureModuleInitializer</c>;
/// a no-op implementation is used when no Redis connection string is configured.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key, or <c>null</c> on a cache miss.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Stores a value under the given key with an optional expiration.
    /// </summary>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a single cache entry.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes every cache entry whose key starts with <paramref name="prefix"/>.
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}
