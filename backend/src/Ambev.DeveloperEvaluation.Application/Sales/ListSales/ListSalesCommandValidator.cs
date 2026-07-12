using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Validator for ListSalesCommand
/// </summary>
public class ListSalesCommandValidator : AbstractValidator<ListSalesCommand>
{
    public ListSalesCommandValidator()
    {
        RuleFor(command => command.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

        RuleFor(command => command.Size)
            .InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100.");

        When(command => command.MinTotalAmount.HasValue && command.MaxTotalAmount.HasValue, () =>
        {
            RuleFor(command => command.MinTotalAmount)
                .LessThanOrEqualTo(command => command.MaxTotalAmount)
                .WithMessage("Minimum total amount cannot be greater than the maximum.");
        });

        When(command => command.MinSaleDate.HasValue && command.MaxSaleDate.HasValue, () =>
        {
            RuleFor(command => command.MinSaleDate)
                .LessThanOrEqualTo(command => command.MaxSaleDate)
                .WithMessage("Minimum sale date cannot be later than the maximum.");
        });
    }
}
