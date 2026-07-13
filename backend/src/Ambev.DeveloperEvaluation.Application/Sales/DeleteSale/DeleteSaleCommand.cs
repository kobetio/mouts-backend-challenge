using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command for permanently deleting a sale record. Unlike <c>CancelSale</c> (which preserves
/// history), this removes the row entirely — it exists to complete the full CRUD surface,
/// not as a business-rule-driven operation.
/// </summary>
public record DeleteSaleCommand : IRequest<DeleteSaleResponse>
{
    /// <summary>
    /// The unique identifier of the sale to delete.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of DeleteSaleCommand
    /// </summary>
    /// <param name="id">The Id of the sale to delete</param>
    public DeleteSaleCommand(Guid id)
    {
        Id = id;
    }
}
