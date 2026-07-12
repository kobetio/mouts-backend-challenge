using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Given active sale When cancelled Then status is Cancelled and event is raised")]
    public void Cancel_ActiveSale_SetsCancelledStatusAndRaisesEvent()
    {
        var sale = SaleTestData.GenerateValidSale();

        sale.Cancel();

        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.DomainEvents.Should().ContainSingle(e => e is SaleCancelledEvent);
    }

    [Fact(DisplayName = "Given cancelled sale When cancelled again Then throws DomainException")]
    public void Cancel_AlreadyCancelledSale_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.Cancel();

        act.Should().Throw<DomainException>()
            .WithMessage("*already cancelled*");
    }

    [Fact(DisplayName = "Given active item When cancelled Then item is excluded from total and event is raised")]
    public void CancelItem_ActiveItem_ExcludesFromTotalAndRaisesEvent()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems((2, 10m), (3, 20m));
        var totalBeforeCancel = sale.TotalAmount;
        var itemToCancel = sale.Items[0];

        sale.CancelItem(itemToCancel.Id);

        itemToCancel.Status.Should().Be(SaleItemStatus.Cancelled);
        sale.TotalAmount.Should().Be(totalBeforeCancel - itemToCancel.TotalAmount);
        sale.Items.Should().Contain(itemToCancel);
        sale.DomainEvents.Should().ContainSingle(e => e is ItemCancelledEvent);
    }

    [Fact(DisplayName = "Given cancelled item When cancelled again Then throws DomainException")]
    public void CancelItem_AlreadyCancelledItem_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        var item = sale.Items[0];
        sale.CancelItem(item.Id);

        var act = () => sale.CancelItem(item.Id);

        act.Should().Throw<DomainException>()
            .WithMessage($"*Item {item.Id} is already cancelled*");
    }

    [Fact(DisplayName = "Given cancelled sale When adding item Then throws DomainException")]
    public void AddItem_CancelledSale_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.AddItem(
            SaleTestData.GenerateProductReference(),
            quantity: 1,
            unitPrice: 10m,
            new TieredDiscountPolicy());

        act.Should().Throw<DomainException>()
            .WithMessage("*already cancelled*");
    }

    [Theory(DisplayName = "Given quantity When adding item Then applies correct discount tier to line total")]
    [InlineData(3, 0.00, 300)]
    [InlineData(4, 0.10, 360)]
    [InlineData(10, 0.20, 800)]
    public void AddItem_AppliesDiscountTierToLineTotal(int quantity, decimal expectedDiscount, decimal expectedTotal)
    {
        var sale = SaleTestData.GenerateSaleWithQuantity(quantity, unitPrice: 100m);
        var item = sale.Items[0];

        item.DiscountPercentage.Should().Be(expectedDiscount);
        item.TotalAmount.Should().Be(expectedTotal);
        sale.TotalAmount.Should().Be(expectedTotal);
    }
}
