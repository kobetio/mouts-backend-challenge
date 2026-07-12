using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling an entire sale. Per the project's business rules (§2.3), cancellation
/// preserves history: the sale remains queryable, but is excluded from active totals/reports.
/// </summary>
public record CancelSaleCommand : IRequest<SaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to cancel.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand
    /// </summary>
    /// <param name="id">The Id of the sale to cancel</param>
    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
