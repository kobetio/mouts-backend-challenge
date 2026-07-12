using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record: a customer purchase, made at a branch, composed of one or more
/// <see cref="SaleItem"/>s. Customer and Branch are entities from other domains, referenced only
/// through the "External Identities" pattern (external Id + denormalized name).
/// </summary>
/// <remarks>
/// This entity currently exposes only its persistence shape (properties + EF Core mapping).
/// Aggregate behavior (adding items, cancelling, recalculating totals, discount rules, domain
/// events) is implemented in the domain/business-rules phase that follows the initial data model.
/// </remarks>
public class Sale : BaseEntity
{
    /// <summary>
    /// Sequential, human-readable sale number (database-generated identity, distinct from <see cref="BaseEntity.Id"/>).
    /// </summary>
    public int SaleNumber { get; set; }

    /// <summary>
    /// The date and time the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// External reference to the customer who made the purchase.
    /// </summary>
    public CustomerReference Customer { get; set; } = null!;

    /// <summary>
    /// External reference to the branch where the sale took place.
    /// </summary>
    public BranchReference Branch { get; set; } = null!;

    /// <summary>
    /// Sum of the <see cref="SaleItem.TotalAmount"/> of all non-cancelled items.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether the entire sale has been cancelled. A cancelled sale is excluded from active
    /// totals/reports but remains queryable for audit purposes.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// The items sold as part of this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// The date and time when the sale record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time of the last update to the sale (e.g. an item cancellation).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public Sale()
    {
        SaleDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        Status = SaleStatus.NotCancelled;
    }
}
