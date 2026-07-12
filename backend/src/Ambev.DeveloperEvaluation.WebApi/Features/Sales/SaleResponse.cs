using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// API response shape for a sale (with its items). Returned by every Sales endpoint that
/// reads or mutates a sale.
/// </summary>
public class SaleResponse
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Sequential sale number (identity column).
    /// </summary>
    public int SaleNumber { get; set; }

    /// <summary>
    /// The date the sale took place.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// External reference to the customer (Id + denormalized name).
    /// </summary>
    public ExternalReferenceResponse Customer { get; set; } = null!;

    /// <summary>
    /// External reference to the branch where the sale took place (Id + denormalized name).
    /// </summary>
    public ExternalReferenceResponse Branch { get; set; } = null!;

    /// <summary>
    /// Total amount of the sale (sum of active, non-cancelled items).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Cancellation status of the sale.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// The items in this sale.
    /// </summary>
    public List<SaleItemResponse> Items { get; set; } = new();

    /// <summary>
    /// When this sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this sale was last updated, if ever.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
