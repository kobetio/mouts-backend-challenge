namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Structured pagination/sorting/filtering parameters for <see cref="ISaleRepository.ListAsync"/>.
/// Translating raw query-string parameters (§3.7: <c>_page</c>, <c>_size</c>, <c>_order</c>,
/// field filters) into this shape is the API layer's responsibility.
/// </summary>
public class SaleListFilter
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    /// <summary>
    /// Comma-separated list of "field direction" clauses (e.g. "saleDate desc, saleNumber asc").
    /// Recognized fields: saleNumber, saleDate/date, totalAmount, status, customerName, branchName.
    /// Unrecognized fields fall back to sorting by sale date.
    /// </summary>
    public string? OrderBy { get; set; }

    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Filters by customer name. Supports the "*" wildcard before/after the value for
    /// partial matches (e.g. "John*"), per §3.7.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Filters by branch name. Supports the "*" wildcard before/after the value for
    /// partial matches, per §3.7.
    /// </summary>
    public string? BranchName { get; set; }

    public bool? IsCancelled { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }
}
