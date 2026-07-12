using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for replacing an existing sale's details and items (full-replacement PUT semantics).
/// </summary>
public class UpdateSaleCommand : IRequest<SaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to update.
    /// </summary>
    public Guid Id { get; set; }

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
    /// The full desired set of items for this sale (see <see cref="UpdateSaleItemCommand"/>).
    /// </summary>
    public List<UpdateSaleItemCommand> Items { get; set; } = new();
}
