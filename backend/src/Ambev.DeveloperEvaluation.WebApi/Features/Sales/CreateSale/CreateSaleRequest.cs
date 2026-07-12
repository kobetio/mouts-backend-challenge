namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Request body for creating a new sale. Follows the "External Identities" pattern: customer,
/// branch and each item's product are referenced by their external Id plus a denormalized name.
/// </summary>
public class CreateSaleRequest
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
    /// The date the sale took place. Defaults to now (UTC) when not provided.
    /// </summary>
    public DateTime? SaleDate { get; set; }

    /// <summary>
    /// The items being sold. A sale must contain at least one item.
    /// </summary>
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}
