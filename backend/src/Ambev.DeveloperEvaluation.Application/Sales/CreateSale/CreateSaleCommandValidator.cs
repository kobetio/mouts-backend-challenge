using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validates the structural shape of a <see cref="CreateSaleCommand"/> (required fields, basic
/// bounds). Business-rule invariants that depend on domain behavior (e.g. the maximum of 20
/// identical items per product) are enforced by the <c>Sale</c> aggregate itself when the
/// handler builds it, not duplicated here.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("Customer Id is required.");

        RuleFor(command => command.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name cannot be longer than 100 characters.");

        RuleFor(command => command.BranchId)
            .NotEqual(Guid.Empty).WithMessage("Branch Id is required.");

        RuleFor(command => command.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(100).WithMessage("Branch name cannot be longer than 100 characters.");

        RuleFor(command => command.Items)
            .NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(command => command.Items).SetValidator(new CreateSaleItemCommandValidator());
    }
}

/// <summary>
/// Validates a single <see cref="CreateSaleItemCommand"/> line.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEqual(Guid.Empty).WithMessage("Product Id is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot be longer than 100 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Item unit price must be greater than zero.");
    }
}
