namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request body for replacing an existing sale's details and items (full-replacement PUT semantics).
/// The sale Id comes from the route, not the body.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// The external identifier of the customer (owned by the Customer domain).
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Denormalized customer name, captured at the time of the sale.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The external identifier of the branch where the sale took place (owned by the Branch domain).
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Denormalized branch name, captured at the time of the sale.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The date the sale took place.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The full desired set of items for this sale.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}
