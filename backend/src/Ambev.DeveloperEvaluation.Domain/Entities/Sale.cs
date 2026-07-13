using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record: a customer purchase, made at a branch, composed of one or more
/// <see cref="SaleItem"/>s. Customer and Branch are entities from other domains, referenced only
/// through the "External Identities" pattern (external Id + denormalized name).
/// </summary>
/// <remarks>
/// This is the aggregate root for the Sales domain: all invariants involving items (discount
/// tiers, cancellation, totals) must be enforced here, never mutated directly from outside.
/// </remarks>
public class Sale : BaseEntity
{
    private readonly List<object> _domainEvents = new();

    /// <summary>
    /// Sequential, human-readable sale number (database-generated identity, distinct from <see cref="BaseEntity.Id"/>).
    /// </summary>
    public int SaleNumber { get; set; }

    /// <summary>
    /// The date and time the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// External reference to the customer who made the purchase.
    /// </summary>
    public CustomerReference Customer { get; set; } = null!;

    /// <summary>
    /// External reference to the branch where the sale took place.
    /// </summary>
    public BranchReference Branch { get; set; } = null!;

    /// <summary>
    /// Sum of the <see cref="SaleItem.TotalAmount"/> of all non-cancelled items.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether the entire sale has been cancelled. A cancelled sale is excluded from active
    /// totals/reports but remains queryable for audit purposes.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// The items sold as part of this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// The date and time when the sale record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time of the last update to the sale (e.g. an item cancellation).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Domain events raised by this aggregate but not yet dispatched by the application layer.
    /// Callers should read and then call <see cref="ClearDomainEvents"/> after dispatching them.
    /// </summary>
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public Sale()
    {
        SaleDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        Status = SaleStatus.NotCancelled;
    }

    /// <summary>
    /// Creates a new sale for the given customer and branch, raising <see cref="SaleCreatedEvent"/>.
    /// This is the preferred entry point for creating a sale (as opposed to the parameterless
    /// constructor, which also serves EF Core materialization and must not raise events).
    /// </summary>
    public static Sale Create(CustomerReference customer, BranchReference branch, DateTime? saleDate = null)
    {
        var sale = new Sale
        {
            Customer = customer,
            Branch = branch,
            SaleDate = saleDate ?? DateTime.UtcNow
        };

        sale.AddDomainEvent(new SaleCreatedEvent(sale));
        return sale;
    }

    /// <summary>
    /// Adds a new item line to the sale, resolving its discount via <paramref name="discountPolicy"/>
    /// and recalculating the sale's total.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if the sale is already cancelled, the quantity is not positive, or the discount
    /// policy rejects the quantity (e.g. more than 20 identical units).
    /// </exception>
    public SaleItem AddItem(ProductReference product, int quantity, decimal unitPrice, IDiscountPolicy discountPolicy)
    {
        EnsureNotCancelled();
        EnsurePositiveQuantity(quantity);

        var discountPercentage = discountPolicy.GetDiscountPercentage(quantity);
        var item = new SaleItem
        {
            SaleId = Id,
            Product = product,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountPercentage = discountPercentage,
            TotalAmount = CalculateItemTotal(quantity, unitPrice, discountPercentage)
        };

        Items.Add(item);
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;

        return item;
    }

    /// <summary>
    /// Updates an existing, non-cancelled item's quantity/unit price, re-resolving its discount
    /// and recalculating the sale's total.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if the sale is cancelled, the item does not exist, the item is cancelled, the
    /// quantity is not positive, or the discount policy rejects the new quantity.
    /// </exception>
    public void UpdateItem(Guid itemId, int quantity, decimal unitPrice, IDiscountPolicy discountPolicy)
    {
        EnsureNotCancelled();
        EnsurePositiveQuantity(quantity);

        var item = GetActiveItemOrThrow(itemId);
        var discountPercentage = discountPolicy.GetDiscountPercentage(quantity);

        item.Quantity = quantity;
        item.UnitPrice = unitPrice;
        item.DiscountPercentage = discountPercentage;
        item.TotalAmount = CalculateItemTotal(quantity, unitPrice, discountPercentage);
        item.UpdatedAt = DateTime.UtcNow;

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an item line entirely from the sale (as opposed to cancelling it, which preserves
    /// its history). Intended for reconciling the item list during a sale update.
    /// </summary>
    /// <exception cref="DomainException">Thrown if the sale is cancelled or the item does not exist.</exception>
    public void RemoveItem(Guid itemId)
    {
        EnsureNotCancelled();

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            throw new DomainException($"Item {itemId} was not found in this sale.");
        }

        Items.Remove(item);
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels an individual item, excluding it from the sale's active total. The item's history
    /// is preserved. Raises <see cref="ItemCancelledEvent"/>.
    /// </summary>
    /// <exception cref="DomainException">Thrown if the sale, or the item itself, is already cancelled, or the item does not exist.</exception>
    public void CancelItem(Guid itemId)
    {
        EnsureNotCancelled();

        var item = GetActiveItemOrThrow(itemId);

        item.Status = SaleItemStatus.Cancelled;
        item.UpdatedAt = DateTime.UtcNow;

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ItemCancelledEvent(this, item));
    }

    /// <summary>
    /// Cancels the entire sale. Items' individual history is preserved; the sale remains
    /// queryable for audit purposes but is excluded from active totals/reports. Raises
    /// <see cref="SaleCancelledEvent"/>.
    /// </summary>
    /// <exception cref="DomainException">Thrown if the sale is already cancelled.</exception>
    public void Cancel()
    {
        EnsureNotCancelled();

        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleCancelledEvent(this));
    }

    /// <summary>
    /// Recalculates <see cref="TotalAmount"/> as the sum of the <see cref="SaleItem.TotalAmount"/>
    /// of all non-cancelled items.
    /// </summary>
    public void RecalculateTotal()
    {
        TotalAmount = Items
            .Where(item => item.Status == SaleItemStatus.NotCancelled)
            .Sum(item => item.TotalAmount);
    }

    /// <summary>
    /// Registers a domain event to be dispatched later by the application layer (e.g. after an
    /// update use case reconciles multiple item changes and wants to raise a single
    /// <see cref="SaleModifiedEvent"/>).
    /// </summary>
    public void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all pending domain events. Should be called by the application layer once the
    /// events currently in <see cref="DomainEvents"/> have been dispatched/logged.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Performs validation of the sale entity (and its items) using <see cref="SaleValidator"/>.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    private SaleItem GetActiveItemOrThrow(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            throw new DomainException($"Item {itemId} was not found in this sale.");
        }

        if (item.Status == SaleItemStatus.Cancelled)
        {
            throw new DomainException($"Item {itemId} is already cancelled.");
        }

        return item;
    }

    private void EnsureNotCancelled()
    {
        if (Status == SaleStatus.Cancelled)
        {
            throw new DomainException("This sale is already cancelled.");
        }
    }

    private static void EnsurePositiveQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Item quantity must be greater than zero.");
        }
    }

    private static decimal CalculateItemTotal(int quantity, decimal unitPrice, decimal discountPercentage)
        => quantity * unitPrice * (1 - discountPercentage);
}
