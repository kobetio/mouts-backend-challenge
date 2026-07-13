using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using System.Globalization;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Cache key conventions and TTLs for Sales read-through caching.
/// </summary>
public static class SaleCacheKeys
{
    public const string ListPrefix = "sales:list:";
    public const string ItemPrefix = "sales:item:";

    /// <summary>Single-sale entries are invalidated on every write, so a longer TTL is safe.</summary>
    public static readonly TimeSpan ItemTtl = TimeSpan.FromMinutes(5);

    /// <summary>List entries use a short TTL because of the combinatorial nature of filters.</summary>
    public static readonly TimeSpan ListTtl = TimeSpan.FromSeconds(60);

    public static string Item(Guid id) => $"{ItemPrefix}{id:D}";

    /// <summary>
    /// Builds a deterministic cache key from every pagination/sorting/filtering field on the
    /// list query, so each distinct <c>GET /sales</c> request gets its own entry.
    /// </summary>
    public static string List(ListSalesCommand command)
    {
        var parts = new[]
        {
            command.Page.ToString(CultureInfo.InvariantCulture),
            command.Size.ToString(CultureInfo.InvariantCulture),
            command.OrderBy ?? string.Empty,
            command.CustomerId?.ToString("D") ?? string.Empty,
            command.BranchId?.ToString("D") ?? string.Empty,
            command.CustomerName ?? string.Empty,
            command.BranchName ?? string.Empty,
            command.IsCancelled?.ToString() ?? string.Empty,
            command.MinTotalAmount?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            command.MaxTotalAmount?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            command.MinSaleDate?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty,
            command.MaxSaleDate?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty
        };

        return ListPrefix + string.Join(':', parts);
    }
}
