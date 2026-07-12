using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IDiscountPolicy discountPolicy,
        IEventPublisher eventPublisher,
        ICacheService cacheService)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _discountPolicy = discountPolicy;
        _eventPublisher = eventPublisher;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request: reconciles the sale's items against the requested
    /// full item list (add/update/remove), then persists the changes and raises SaleModifiedEvent.
    /// </summary>
    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        sale.Customer = new CustomerReference(command.CustomerId, command.CustomerName);
        sale.Branch = new BranchReference(command.BranchId, command.BranchName);
        sale.SaleDate = command.SaleDate;

        ReconcileItems(sale, command.Items);

        var domainValidation = sale.Validate();
        if (!domainValidation.IsValid)
        {
            throw new DomainException(string.Join("; ", domainValidation.Errors.Select(e => e.Detail)));
        }

        sale.AddDomainEvent(new SaleModifiedEvent(sale));

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await updatedSale.PublishDomainEventsAsync(_eventPublisher, cancellationToken);
        await _cacheService.InvalidateSalesCachesAsync(updatedSale.Id, cancellationToken);

        return _mapper.Map<SaleResult>(updatedSale);
    }

    private void ReconcileItems(Domain.Entities.Sale sale, List<UpdateSaleItemCommand> requestedItems)
    {
        var requestedExistingIds = requestedItems
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToHashSet();

        var idsToRemove = sale.Items
            .Where(item => !requestedExistingIds.Contains(item.Id))
            .Select(item => item.Id)
            .ToList();

        foreach (var itemId in idsToRemove)
        {
            sale.RemoveItem(itemId);
        }

        foreach (var requestedItem in requestedItems)
        {
            var matchesExistingItem = requestedItem.Id.HasValue
                && sale.Items.Any(item => item.Id == requestedItem.Id.Value);

            if (matchesExistingItem)
            {
                sale.UpdateItem(requestedItem.Id!.Value, requestedItem.Quantity, requestedItem.UnitPrice, _discountPolicy);
            }
            else
            {
                sale.AddItem(
                    new ProductReference(requestedItem.ProductId, requestedItem.ProductName),
                    requestedItem.Quantity,
                    requestedItem.UnitPrice,
                    _discountPolicy);
            }
        }
    }
}
