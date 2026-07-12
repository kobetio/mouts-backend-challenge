using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a single product line within a <see cref="Sale"/>. The product is an entity from
/// another domain, referenced only through the "External Identities" pattern (external Id +
/// denormalized name).
/// </summary>
/// <remarks>
/// This entity currently exposes only its persistence shape (properties + EF Core mapping).
/// Discount calculation and cancellation behavior are implemented in the domain/business-rules
/// phase that follows the initial data model.
/// </remarks>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// The Id of the <see cref="Sale"/> this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// External reference to the product sold.
    /// </summary>
    public ProductReference Product { get; set; } = null!;

    /// <summary>
    /// The quantity sold of this product within the sale. Drives the discount tier (see business rules).
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The product's unit price at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount percentage applied to this item, as a fraction (e.g. 0.10 for 10%), based on the quantity tier.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// The item's total amount: <c>Quantity * UnitPrice * (1 - DiscountPercentage)</c>.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether this specific item has been cancelled. A cancelled item is excluded from the
    /// sale's active total but remains queryable for audit purposes.
    /// </summary>
    public SaleItemStatus Status { get; set; }

    /// <summary>
    /// The date and time when the item record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time of the last update to the item (e.g. its cancellation).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public SaleItem()
    {
        CreatedAt = DateTime.UtcNow;
        Status = SaleItemStatus.NotCancelled;
    }
}
