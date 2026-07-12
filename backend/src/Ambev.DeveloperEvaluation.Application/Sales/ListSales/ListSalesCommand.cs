using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving a paginated, sorted, filtered list of sales (§3.7). Translating raw
/// query-string parameters (<c>_page</c>, <c>_size</c>, <c>_order</c>, field filters) into this
/// structured shape is the API layer's responsibility (see Phase 6).
/// </summary>
public class ListSalesCommand : IRequest<ListSalesResult>
{
    /// <summary>
    /// Page number (1-based). Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10.
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Comma-separated "field direction" ordering clauses (e.g. "saleDate desc, saleNumber asc").
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Filters results to sales for a specific customer.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Filters results to sales at a specific branch.
    /// </summary>
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Filters results by customer name. Supports the "*" wildcard before/after the value
    /// for partial matches (e.g. "John*"), per §3.7.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Filters results by branch name. Supports the "*" wildcard before/after the value
    /// for partial matches, per §3.7.
    /// </summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// Filters results by cancellation status.
    /// </summary>
    public bool? IsCancelled { get; set; }

    /// <summary>
    /// Filters results to sales with a total amount greater than or equal to this value.
    /// </summary>
    public decimal? MinTotalAmount { get; set; }

    /// <summary>
    /// Filters results to sales with a total amount less than or equal to this value.
    /// </summary>
    public decimal? MaxTotalAmount { get; set; }

    /// <summary>
    /// Filters results to sales on or after this date.
    /// </summary>
    public DateTime? MinSaleDate { get; set; }

    /// <summary>
    /// Filters results to sales on or before this date.
    /// </summary>
    public DateTime? MaxSaleDate { get; set; }
}
