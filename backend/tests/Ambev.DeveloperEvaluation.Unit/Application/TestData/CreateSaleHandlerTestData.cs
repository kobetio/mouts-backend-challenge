using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Bogus-based test data for Sales application command handlers.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand> CommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.CustomerId, _ => Guid.NewGuid())
        .RuleFor(c => c.CustomerName, f => f.Company.CompanyName())
        .RuleFor(c => c.BranchId, _ => Guid.NewGuid())
        .RuleFor(c => c.BranchName, f => f.Commerce.Department())
        .RuleFor(c => c.SaleDate, f => f.Date.Recent(7))
        .RuleFor(c => c.Items, f => new List<CreateSaleItemCommand>
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                ProductName = f.Commerce.ProductName(),
                Quantity = f.Random.Int(1, 3),
                UnitPrice = f.Random.Decimal(1, 100)
            }
        });

    public static CreateSaleCommand GenerateValidCommand()
        => CommandFaker.Generate();

    public static CreateSaleCommand GenerateCommandWithQuantity(int quantity, decimal unitPrice = 10m)
    {
        var command = CommandFaker.Generate();
        command.Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                Quantity = quantity,
                UnitPrice = unitPrice
            }
        ];
        return command;
    }
}
