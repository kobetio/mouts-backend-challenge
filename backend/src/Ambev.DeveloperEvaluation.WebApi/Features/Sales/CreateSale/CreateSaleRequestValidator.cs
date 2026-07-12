using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validates the structural shape of a <see cref="CreateSaleRequest"/>.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("Customer Id is required.");

        RuleFor(request => request.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name cannot be longer than 100 characters.");

        RuleFor(request => request.BranchId)
            .NotEqual(Guid.Empty).WithMessage("Branch Id is required.");

        RuleFor(request => request.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(100).WithMessage("Branch name cannot be longer than 100 characters.");

        RuleFor(request => request.Items)
            .NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(request => request.Items).SetValidator(new CreateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validates a single <see cref="CreateSaleItemRequest"/> line.
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    public CreateSaleItemRequestValidator()
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
