using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Read-model shape for a sale (with its items). Shared by every Sales use case
/// (Create/Update/Get/List/Cancel) since they all return the same representation of a sale.
/// </summary>
public class SaleResult
{
    public Guid Id { get; set; }
    public int SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public ExternalReferenceResult Customer { get; set; } = null!;
    public ExternalReferenceResult Branch { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; }
    public List<SaleItemResult> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
