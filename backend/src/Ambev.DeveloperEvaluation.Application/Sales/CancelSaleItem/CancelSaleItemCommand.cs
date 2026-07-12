using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Command for cancelling a single item within a sale. Per the project's business rules
/// (§2.3), cancellation preserves history: the item remains queryable, but is excluded from
/// the sale's active total. Cancelling an already-cancelled item is rejected.
/// </summary>
public record CancelSaleItemCommand : IRequest<SaleResult>
{
    /// <summary>
    /// The unique identifier of the sale that owns the item.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// The unique identifier of the item to cancel.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Initializes a new instance of CancelSaleItemCommand
    /// </summary>
    /// <param name="saleId">The Id of the sale that owns the item</param>
    /// <param name="itemId">The Id of the item to cancel</param>
    public CancelSaleItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}
