using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Command for retrieving a sale by its unique identifier
/// </summary>
public record GetSaleCommand : IRequest<SaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to retrieve
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of GetSaleCommand
    /// </summary>
    /// <param name="id">The Id of the sale to retrieve</param>
    public GetSaleCommand(Guid id)
    {
        Id = id;
    }
}
