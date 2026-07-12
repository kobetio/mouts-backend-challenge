using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.Product)
            .NotNull().WithMessage("Item product reference must be provided.");

        When(item => item.Product != null, () =>
        {
            RuleFor(item => item.Product.Id)
                .NotEqual(Guid.Empty).WithMessage("Item product Id must be provided.");

            RuleFor(item => item.Product.Name)
                .NotEmpty().WithMessage("Item product name must be provided.")
                .MaximumLength(100).WithMessage("Item product name cannot be longer than 100 characters.");
        });

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Item quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items of the same product in a single sale.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Item unit price must be greater than zero.");

        RuleFor(item => item.DiscountPercentage)
            .InclusiveBetween(0, 1).WithMessage("Item discount percentage must be between 0 and 1.");

        RuleFor(item => item.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Item total amount cannot be negative.");
    }
}
