using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.Common.Caching;

/// <summary>
/// DI registration helpers for <see cref="ICacheService"/> implementations.
/// </summary>
public static class CacheServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="RedisCacheService"/> backed by the given Redis connection string.
    /// </summary>
    public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<ICacheService, RedisCacheService>();
        return services;
    }

    /// <summary>
    /// Registers <see cref="NullCacheService"/> (no-op) when Redis is not available.
    /// </summary>
    public static IServiceCollection AddNullCache(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, NullCacheService>();
        return services;
    }
}
