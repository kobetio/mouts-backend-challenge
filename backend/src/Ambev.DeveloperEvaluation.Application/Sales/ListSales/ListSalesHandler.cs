using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSalesCommand request
    /// </summary>
    public async Task<ListSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var filter = new SaleListFilter
        {
            Page = request.Page,
            Size = request.Size,
            OrderBy = request.OrderBy,
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            CustomerName = request.CustomerName,
            BranchName = request.BranchName,
            IsCancelled = request.IsCancelled,
            MinTotalAmount = request.MinTotalAmount,
            MaxTotalAmount = request.MaxTotalAmount,
            MinSaleDate = request.MinSaleDate,
            MaxSaleDate = request.MaxSaleDate
        };

        var pagedResult = await _saleRepository.ListAsync(filter, cancellationToken);

        return new ListSalesResult
        {
            Items = _mapper.Map<IReadOnlyList<SaleResult>>(pagedResult.Items),
            TotalItems = pagedResult.TotalCount,
            CurrentPage = request.Page,
            TotalPages = (int)Math.Ceiling(pagedResult.TotalCount / (double)request.Size)
        };
    }
}
