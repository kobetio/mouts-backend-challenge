using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when an existing sale's details/items are modified (e.g. via the update use case).
/// </summary>
public class SaleModifiedEvent
{
    public Sale Sale { get; }

    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
    }
}
