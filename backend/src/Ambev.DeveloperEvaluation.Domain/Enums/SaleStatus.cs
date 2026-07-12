namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Represents the cancellation status of a sale.
/// A cancelled sale is excluded from active totals/reports but remains queryable for audit purposes.
/// </summary>
public enum SaleStatus
{
    NotCancelled = 0,
    Cancelled = 1
}
