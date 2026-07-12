using Ambev.DeveloperEvaluation.Common.Caching;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Shared cache-invalidation helper invoked by every Sales write handler after a successful
/// persist (see PROJECT.md §3.12).
/// </summary>
internal static class SaleCacheInvalidationExtensions
{
    public static async Task InvalidateSalesCachesAsync(
        this ICacheService cache,
        Guid saleId,
        CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(SaleCacheKeys.Item(saleId), cancellationToken);
        await cache.RemoveByPrefixAsync(SaleCacheKeys.ListPrefix, cancellationToken);
    }
}
