namespace Ambev.DeveloperEvaluation.Domain.Services;

/// <summary>
/// Resolves the discount percentage applicable to a sale item, based on the quantity of
/// identical units of the same product being sold.
/// </summary>
public interface IDiscountPolicy
{
    /// <summary>
    /// Returns the discount percentage (as a fraction, e.g. 0.10 for 10%) for the given quantity.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when <paramref name="quantity"/> exceeds the maximum number of identical items
    /// allowed per product in a single sale.
    /// </exception>
    decimal GetDiscountPercentage(int quantity);
}
