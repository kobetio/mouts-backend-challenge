using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a new sale is created. Per the project's business rules (§1.3), this event does
/// not need to be published to a message broker — it just needs to be raised at a clear point in
/// the code; the application layer logs it (see the Domain Events & Application Logging phase).
/// </summary>
public class SaleCreatedEvent
{
    public Sale Sale { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
