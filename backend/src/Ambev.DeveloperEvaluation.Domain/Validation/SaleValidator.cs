using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleDate)
            .NotEmpty().WithMessage("Sale date must be provided.");

        RuleFor(sale => sale.Customer)
            .NotNull().WithMessage("Sale customer reference must be provided.");

        When(sale => sale.Customer != null, () =>
        {
            RuleFor(sale => sale.Customer.Id)
                .NotEqual(Guid.Empty).WithMessage("Customer Id must be provided.");

            RuleFor(sale => sale.Customer.Name)
                .NotEmpty().WithMessage("Customer name must be provided.")
                .MaximumLength(100).WithMessage("Customer name cannot be longer than 100 characters.");
        });

        RuleFor(sale => sale.Branch)
            .NotNull().WithMessage("Sale branch reference must be provided.");

        When(sale => sale.Branch != null, () =>
        {
            RuleFor(sale => sale.Branch.Id)
                .NotEqual(Guid.Empty).WithMessage("Branch Id must be provided.");

            RuleFor(sale => sale.Branch.Name)
                .NotEmpty().WithMessage("Branch name must be provided.")
                .MaximumLength(100).WithMessage("Branch name cannot be longer than 100 characters.");
        });

        RuleFor(sale => sale.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Sale total amount cannot be negative.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(sale => sale.Items)
            .SetValidator(new SaleItemValidator());
    }
}
