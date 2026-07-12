using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.Common.Events;

/// <summary>
/// Default <see cref="IEventPublisher"/> implementation: logs every domain event through the
/// application's existing Serilog pipeline instead of publishing to a message broker.
/// Structured properties (<c>SaleId</c>, <c>SaleNumber</c>, <c>ItemId</c>) are extracted via
/// reflection so this class stays in <c>Common</c> without referencing the Domain assembly.
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
        var eventType = domainEvent.GetType().Name;
        ExtractEventContext(domainEvent, out var saleId, out var saleNumber, out var itemId);

        _logger.LogInformation(
            "Domain event published: {EventType} | SaleId: {SaleId} | SaleNumber: {SaleNumber} | ItemId: {ItemId}",
            eventType,
            saleId,
            saleNumber,
            itemId);

        return Task.CompletedTask;
    }

    private static void ExtractEventContext(
        object domainEvent,
        out Guid? saleId,
        out int? saleNumber,
        out Guid? itemId)
    {
        saleId = null;
        saleNumber = null;
        itemId = null;

        var eventType = domainEvent.GetType();
        var sale = eventType.GetProperty("Sale")?.GetValue(domainEvent);

        if (sale is not null)
        {
            saleId = sale.GetType().GetProperty("Id")?.GetValue(sale) as Guid?;
            saleNumber = sale.GetType().GetProperty("SaleNumber")?.GetValue(sale) as int?;
        }

        var item = eventType.GetProperty("Item")?.GetValue(domainEvent);
        if (item is not null)
        {
            itemId = item.GetType().GetProperty("Id")?.GetValue(item) as Guid?;
        }
    }
}
