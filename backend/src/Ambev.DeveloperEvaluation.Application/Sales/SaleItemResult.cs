using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Read-model shape for a sale item. Shared by every Sales use case (Create/Update/Get/List/Cancel)
/// since they all return the same representation of a sale.
/// </summary>
public class SaleItemResult
{
    public Guid Id { get; set; }
    public ExternalReferenceResult Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleItemStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
