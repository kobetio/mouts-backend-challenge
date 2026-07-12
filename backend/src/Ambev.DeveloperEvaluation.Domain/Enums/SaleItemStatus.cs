namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Represents the cancellation status of an individual sale item.
/// A cancelled item is excluded from the sale's active total but remains queryable for audit purposes.
/// </summary>
public enum SaleItemStatus
{
    NotCancelled = 0,
    Cancelled = 1
}
