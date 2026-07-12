using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides Bogus-based test data for <see cref="Sale"/> and <see cref="SaleItem"/> entities,
/// mirroring <see cref="UserTestData"/>.
/// </summary>
public static class SaleTestData
{
    private static readonly TieredDiscountPolicy DiscountPolicy = new();

    private static readonly Faker Faker = new();

    /// <summary>
    /// Creates a valid sale with one randomly generated item line (quantity 1–3, no discount tier).
    /// </summary>
    public static Sale GenerateValidSale(int itemQuantity = 2, decimal unitPrice = 10m)
    {
        var sale = Sale.Create(
            new CustomerReference(Guid.NewGuid(), Faker.Company.CompanyName()),
            new BranchReference(Guid.NewGuid(), Faker.Commerce.Department()));

        sale.Id = Guid.NewGuid();

        var item = sale.AddItem(
            new ProductReference(Guid.NewGuid(), Faker.Commerce.ProductName()),
            itemQuantity,
            unitPrice,
            DiscountPolicy);

        item.Id = Guid.NewGuid();

        sale.ClearDomainEvents();
        return sale;
    }

    /// <summary>
    /// Creates a valid sale with multiple item lines.
    /// </summary>
    public static Sale GenerateValidSaleWithItems(params (int quantity, decimal unitPrice)[] lines)
    {
        var sale = Sale.Create(
            new CustomerReference(Guid.NewGuid(), Faker.Company.CompanyName()),
            new BranchReference(Guid.NewGuid(), Faker.Commerce.Department()));

        sale.Id = Guid.NewGuid();

        foreach (var (quantity, unitPrice) in lines)
        {
            var item = sale.AddItem(
                new ProductReference(Guid.NewGuid(), Faker.Commerce.ProductName()),
                quantity,
                unitPrice,
                DiscountPolicy);

            item.Id = Guid.NewGuid();
        }

        sale.ClearDomainEvents();
        return sale;
    }

    /// <summary>
    /// Creates a sale with a specific quantity on a single product line (for discount tier tests).
    /// </summary>
    public static Sale GenerateSaleWithQuantity(int quantity, decimal unitPrice = 100m)
    {
        var sale = Sale.Create(
            new CustomerReference(Guid.NewGuid(), "Test Customer"),
            new BranchReference(Guid.NewGuid(), "Test Branch"));

        sale.Id = Guid.NewGuid();

        var item = sale.AddItem(
            new ProductReference(Guid.NewGuid(), "Test Product"),
            quantity,
            unitPrice,
            DiscountPolicy);

        item.Id = Guid.NewGuid();

        sale.ClearDomainEvents();
        return sale;
    }

    public static ProductReference GenerateProductReference()
        => new(Guid.NewGuid(), Faker.Commerce.ProductName());

    public static CustomerReference GenerateCustomerReference()
        => new(Guid.NewGuid(), Faker.Company.CompanyName());

    public static BranchReference GenerateBranchReference()
        => new(Guid.NewGuid(), Faker.Commerce.Department());
}
