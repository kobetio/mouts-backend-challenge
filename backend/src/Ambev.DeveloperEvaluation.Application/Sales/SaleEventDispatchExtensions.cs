using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Shared helper for publishing and clearing the domain events collected on a
/// <see cref="Sale"/> aggregate after a use case successfully persists its changes.
/// </summary>
internal static class SaleEventDispatchExtensions
{
    public static async Task PublishDomainEventsAsync(
        this Sale sale,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in sale.DomainEvents)
        {
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        sale.ClearDomainEvents();
    }
}
