namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Publishes domain events. This is an architectural stand-in for a real message broker
/// (e.g. RabbitMQ, Rebus). Publishing to a real broker is optional for this challenge;
/// a clear, observable log entry is enough.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a single domain event.
    /// </summary>
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}
