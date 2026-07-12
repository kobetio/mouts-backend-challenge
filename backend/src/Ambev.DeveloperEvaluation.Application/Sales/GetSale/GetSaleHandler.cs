using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleCommand requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper, ICacheService cacheService)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Handles the GetSaleCommand request
    /// </summary>
    public async Task<SaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var cacheKey = SaleCacheKeys.Item(request.Id);
        var cached = await _cacheService.GetAsync<SaleResult>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        var result = _mapper.Map<SaleResult>(sale);
        await _cacheService.SetAsync(cacheKey, result, SaleCacheKeys.ItemTtl, cancellationToken);

        return result;
    }
}
