using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a new sale is created. This event does not need to be published to a message
/// broker — it just needs to be raised at a clear point in the code; the application layer logs it.
/// </summary>
public class SaleCreatedEvent
{
    public Sale Sale { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
