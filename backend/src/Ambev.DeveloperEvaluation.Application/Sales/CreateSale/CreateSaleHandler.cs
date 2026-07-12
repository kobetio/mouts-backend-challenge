using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;

    public CreateSaleHandler(
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
    /// Handles the CreateSaleCommand request: builds the Sale aggregate (which applies the
    /// tiered discount policy and raises SaleCreatedEvent), persists it, and publishes its
    /// domain events.
    /// </summary>
    public async Task<SaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = Sale.Create(
            new CustomerReference(command.CustomerId, command.CustomerName),
            new BranchReference(command.BranchId, command.BranchName),
            command.SaleDate);

        foreach (var item in command.Items)
        {
            sale.AddItem(
                new ProductReference(item.ProductId, item.ProductName),
                item.Quantity,
                item.UnitPrice,
                _discountPolicy);
        }

        var domainValidation = sale.Validate();
        if (!domainValidation.IsValid)
        {
            throw new DomainException(string.Join("; ", domainValidation.Errors.Select(e => e.Detail)));
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        await createdSale.PublishDomainEventsAsync(_eventPublisher, cancellationToken);
        await _cacheService.InvalidateSalesCachesAsync(createdSale.Id, cancellationToken);

        return _mapper.Map<SaleResult>(createdSale);
    }
}
