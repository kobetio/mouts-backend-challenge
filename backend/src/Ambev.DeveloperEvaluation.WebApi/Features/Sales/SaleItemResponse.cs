using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// API response shape for a single sale item line.
/// </summary>
public class SaleItemResponse
{
    /// <summary>
    /// The unique identifier of this sale item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// External reference to the product sold (Id + denormalized name).
    /// </summary>
    public ExternalReferenceResponse Product { get; set; } = null!;

    /// <summary>
    /// Number of identical units of this product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount percentage applied (as a fraction, e.g. 0.10 for 10%).
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Total amount for this line after discount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Cancellation status of this item.
    /// </summary>
    public SaleItemStatus Status { get; set; }

    /// <summary>
    /// When this item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this item was last updated, if ever.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
