using Ambev.DeveloperEvaluation.Domain.Services;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class TieredDiscountPolicyTests
{
    private readonly TieredDiscountPolicy _policy = new();

    [Theory(DisplayName = "Given quantity below 4 When resolving discount Then returns 0%")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetDiscountPercentage_BelowFourUnits_ReturnsZero(int quantity)
    {
        _policy.GetDiscountPercentage(quantity).Should().Be(0m);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When resolving discount Then returns 10%")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void GetDiscountPercentage_BetweenFourAndNine_ReturnsTenPercent(int quantity)
    {
        _policy.GetDiscountPercentage(quantity).Should().Be(0.10m);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When resolving discount Then returns 20%")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void GetDiscountPercentage_BetweenTenAndTwenty_ReturnsTwentyPercent(int quantity)
    {
        _policy.GetDiscountPercentage(quantity).Should().Be(0.20m);
    }

    [Fact(DisplayName = "Given quantity above 20 When resolving discount Then throws DomainException")]
    public void GetDiscountPercentage_AboveTwenty_ThrowsDomainException()
    {
        var act = () => _policy.GetDiscountPercentage(21);

        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot sell more than 20 identical items*");
    }
}
