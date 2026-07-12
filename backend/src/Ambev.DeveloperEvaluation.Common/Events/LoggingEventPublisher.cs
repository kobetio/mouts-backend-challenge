using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Default <see cref="IEventPublisher"/> implementation: logs every domain event through the
/// application's existing Serilog pipeline instead of publishing to a message broker.
/// </summary>
public class LoggingEventPublisher : IEventPublisher
{
    private readonly ILogger<LoggingEventPublisher> _logger;

    public LoggingEventPublisher(ILogger<LoggingEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Domain event published: {EventType} | {@DomainEvent}",
            domainEvent.GetType().Name,
            domainEvent);

        return Task.CompletedTask;
    }
}
