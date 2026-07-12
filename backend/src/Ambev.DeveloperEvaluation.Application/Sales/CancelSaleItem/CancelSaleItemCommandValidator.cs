using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Validator for CancelSaleItemCommand
/// </summary>
public class CancelSaleItemCommandValidator : AbstractValidator<CancelSaleItemCommand>
{
    public CancelSaleItemCommandValidator()
    {
        RuleFor(command => command.SaleId)
            .NotEqual(Guid.Empty).WithMessage("Sale Id is required.");

        RuleFor(command => command.ItemId)
            .NotEqual(Guid.Empty).WithMessage("Item Id is required.");
    }
}
