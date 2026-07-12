using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Caching;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new GetSaleHandler(_saleRepository, _mapper, _cacheService);
    }

    [Fact(DisplayName = "Given cached sale When getting sale Then returns cached result without hitting repository")]
    public async Task Handle_CacheHit_ReturnsCachedResultWithoutRepositoryCall()
    {
        var saleId = Guid.NewGuid();
        var cachedResult = new SaleResult { Id = saleId, SaleNumber = 42 };

        _cacheService
            .GetAsync<SaleResult>(SaleCacheKeys.Item(saleId), Arg.Any<CancellationToken>())
            .Returns(cachedResult);

        var result = await _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        result.Should().BeSameAs(cachedResult);
        await _saleRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _cacheService.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<SaleResult>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cache miss When getting sale Then loads from repository and stores in cache")]
    public async Task Handle_CacheMiss_LoadsFromRepositoryAndStoresInCache()
    {
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = 7,
            Customer = new CustomerReference(Guid.NewGuid(), "Customer A"),
            Branch = new BranchReference(Guid.NewGuid(), "Branch A"),
            Status = SaleStatus.NotCancelled
        };
        var mappedResult = new SaleResult { Id = saleId, SaleNumber = 7 };

        _cacheService
            .GetAsync<SaleResult>(SaleCacheKeys.Item(saleId), Arg.Any<CancellationToken>())
            .Returns((SaleResult?)null);
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(mappedResult);

        var result = await _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        result.Should().BeSameAs(mappedResult);
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _cacheService.Received(1).SetAsync(
            SaleCacheKeys.Item(saleId),
            mappedResult,
            SaleCacheKeys.ItemTtl,
            Arg.Any<CancellationToken>());
    }
}
