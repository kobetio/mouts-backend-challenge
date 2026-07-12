namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// A page of results plus the total count across all pages, as returned by
/// <see cref="ISaleRepository.ListAsync"/>.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
}
