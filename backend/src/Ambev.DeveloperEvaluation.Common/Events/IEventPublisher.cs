namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Publishes domain events. This is an architectural stand-in for a real message broker
/// (e.g. RabbitMQ, Rebus) — per the project's business rules (§1.3), it is not required to
/// actually publish events to a broker; a clear, observable log entry is enough.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a single domain event.
    /// </summary>
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}
