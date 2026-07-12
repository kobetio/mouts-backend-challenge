using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validates the structural shape of an <see cref="UpdateSaleCommand"/>. As with
/// <c>CreateSaleCommandValidator</c>, business-rule invariants enforced by domain behavior
/// (e.g. the 20-item cap) are left to the <c>Sale</c> aggregate itself.
/// </summary>
public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("Sale Id is required.");

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

        RuleForEach(command => command.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}

/// <summary>
/// Validates a single <see cref="UpdateSaleItemCommand"/> line.
/// </summary>
public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
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
