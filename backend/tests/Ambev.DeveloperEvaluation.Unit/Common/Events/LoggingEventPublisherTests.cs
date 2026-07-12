using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Events;

public class LoggingEventPublisherTests
{
    private readonly ILogger<LoggingEventPublisher> _logger;
    private readonly LoggingEventPublisher _publisher;

    public LoggingEventPublisherTests()
    {
        _logger = Substitute.For<ILogger<LoggingEventPublisher>>();
        _publisher = new LoggingEventPublisher(_logger);
    }

    [Theory(DisplayName = "Given a sales domain event When publishing Then logs at Information level")]
    [InlineData(typeof(SaleCreatedEvent))]
    [InlineData(typeof(SaleModifiedEvent))]
    [InlineData(typeof(SaleCancelledEvent))]
    [InlineData(typeof(ItemCancelledEvent))]
    public async Task PublishAsync_SalesDomainEvents_LogsInformation(Type eventType)
    {
        var sale = CreateSampleSale();
        object domainEvent = eventType.Name switch
        {
            nameof(SaleCreatedEvent) => new SaleCreatedEvent(sale),
            nameof(SaleModifiedEvent) => new SaleModifiedEvent(sale),
            nameof(SaleCancelledEvent) => new SaleCancelledEvent(sale),
            nameof(ItemCancelledEvent) => new ItemCancelledEvent(sale, sale.Items[0]),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType))
        };

        await _publisher.PublishAsync(domainEvent);

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    private static Sale CreateSampleSale()
    {
        var sale = Sale.Create(
            new CustomerReference(Guid.NewGuid(), "Test Customer"),
            new BranchReference(Guid.NewGuid(), "Test Branch"));

        sale.AddItem(
            new ProductReference(Guid.NewGuid(), "Test Product"),
            quantity: 2,
            unitPrice: 10m,
            new TieredDiscountPolicy());

        sale.SaleNumber = 1001;
        return sale;
    }
}
