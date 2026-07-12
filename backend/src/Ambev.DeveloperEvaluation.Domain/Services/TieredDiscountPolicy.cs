namespace Ambev.DeveloperEvaluation.Domain.Services;

/// <summary>
/// Default <see cref="IDiscountPolicy"/> implementation enforcing the quantity-based discount
/// tiers from the project's business rules (§2.1):
/// <list type="bullet">
/// <item>Fewer than 4 units: no discount.</item>
/// <item>4 to 9 units: 10% discount.</item>
/// <item>10 to 20 units: 20% discount.</item>
/// <item>More than 20 units: not allowed (rejected).</item>
/// </list>
/// The tiers are evaluated against the quantity of a single sale item line (i.e. how many units
/// of that specific product are being sold in that line), not the sale's overall item count.
/// </summary>
public class TieredDiscountPolicy : IDiscountPolicy
{
    private const int NoDiscountMaxQuantity = 3;
    private const int LowTierMaxQuantity = 9;
    private const int HighTierMaxQuantity = 20;

    private const decimal LowTierDiscount = 0.10m;
    private const decimal HighTierDiscount = 0.20m;

    public decimal GetDiscountPercentage(int quantity)
    {
        if (quantity > HighTierMaxQuantity)
        {
            throw new DomainException(
                $"Cannot sell more than {HighTierMaxQuantity} identical items of the same product in a single sale.");
        }

        if (quantity > LowTierMaxQuantity)
        {
            return HighTierDiscount;
        }

        if (quantity > NoDiscountMaxQuantity)
        {
            return LowTierDiscount;
        }

        return 0m;
    }
}
