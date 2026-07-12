namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Paginated result of a ListSales query. The API layer (Phase 6) maps this to the wire-format
/// response shape required by §3.7 (<c>data</c>/<c>totalItems</c>/<c>currentPage</c>/<c>totalPages</c>).
/// </summary>
public class ListSalesResult
{
    public IReadOnlyList<SaleResult> Items { get; set; } = Array.Empty<SaleResult>();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
