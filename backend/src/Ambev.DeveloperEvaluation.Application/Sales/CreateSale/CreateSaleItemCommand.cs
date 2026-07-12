namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents a single item line to add to a new sale.
/// </summary>
public class CreateSaleItemCommand
{
    /// <summary>
    /// The external identifier of the product (owned by the Product domain).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Denormalized product name, captured at the time of the sale.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The number of identical units of this product being sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
