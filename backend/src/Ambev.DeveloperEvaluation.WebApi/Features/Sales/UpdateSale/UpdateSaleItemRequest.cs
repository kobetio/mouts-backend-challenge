namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a single item line in an update-sale request body. When <see cref="Id"/> is
/// provided and matches an existing item on the sale, that item is updated; otherwise a new
/// item is added. Existing items whose Id is absent from the request are removed.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// The Id of the existing item to update. Null means "add as a new item".
    /// </summary>
    public Guid? Id { get; set; }

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
