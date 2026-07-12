using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when an individual item of a sale is cancelled (see <see cref="Sale.CancelItem"/>).
/// </summary>
public class ItemCancelledEvent
{
    public Sale Sale { get; }
    public SaleItem Item { get; }

    public ItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
    }
}
